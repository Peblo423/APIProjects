using AutoMapper;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Get
        [HttpGet]
        public async Task<List<LibroDto>> Get()
        {
            List<Libro> libros = await _context.Libros.ToListAsync();

            List<LibroDto> librosDto = _mapper.Map<List<LibroDto>>(libros);

            return librosDto;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroConAutoresDto>> Get(int id)
        {
            Libro? libro = await _context.Libros
                .Include(x => x.Autores)
                    .ThenInclude(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libro is null)
            {
                return NotFound();
            }

            LibroConAutoresDto libroConAutoresDto = _mapper.Map<LibroConAutoresDto>(libro);
            return libroConAutoresDto;
        }
        #endregion

        #region Post
        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDto libroCreacionDto)
        {
            // La lista de id de autores del libro ingresado no puede estar vacía
            if(libroCreacionDto.AutoresIds is null || libroCreacionDto.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDto.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            // Guardamos todos los ids del dto que sí encontramos en la tabla Autores
            List<int> autoresIdsExisten = await _context.Autores
                .Where(x => libroCreacionDto.AutoresIds
                .Contains(x.Id)).Select(x => x.Id)
                .ToListAsync();
            
            // Deberíamos haber encontrado todos
            if (autoresIdsExisten.Count != libroCreacionDto.AutoresIds.Count)
            {
                // Guardo los ids no encontrados
                IEnumerable<int> autoresNoExisten = libroCreacionDto.AutoresIds.Except(autoresIdsExisten);

                // Junto ids no encontrados en un string
                string autoresNoExistenString = string.Join(",", autoresNoExisten);
                string mensajeError = $"Los siguientes autores no existen: {autoresNoExistenString}";

                // Muestro ids no encontrados en el mensaje
                ModelState.AddModelError(nameof(libroCreacionDto), mensajeError);

                return ValidationProblem();
            }
            
            Libro libro = _mapper.Map<Libro>(libroCreacionDto);
            AsignarOrdenAutores(libro);
            
            _context.Add(libro);
            await _context.SaveChangesAsync();
            LibroDto libroDto = _mapper.Map<LibroDto>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDto);
        }

        // Función para asignar valores al campo Orden de cada AutorLibro de la lista que guarda el Libro
        private static void AsignarOrdenAutores(Libro libro)
        {
            if(libro is not null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i+1;
                }
            }
        }
        #endregion

        #region Put
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDto libroCreacionDto)
        {
            // La lista de id de autores del libro ingresado no puede estar vacía
            if (libroCreacionDto.AutoresIds is null || libroCreacionDto.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDto.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            // Guardamos todos los ids del dto que sí encontramos en la tabla Autores
            List<int> autoresIdsExisten = await _context.Autores
                .Where(x => libroCreacionDto.AutoresIds
                .Contains(x.Id)).Select(x => x.Id)
                .ToListAsync();

            // Deberíamos haber encontrado todos
            if (autoresIdsExisten.Count != libroCreacionDto.AutoresIds.Count)
            {
                // Guardo los ids no encontrados
                IEnumerable<int> autoresNoExisten = libroCreacionDto.AutoresIds.Except(autoresIdsExisten);

                // Junto ids no encontrados en un string
                string autoresNoExistenString = string.Join(",", autoresNoExisten);
                string mensajeError = $"Los siguientes autores no existen: {autoresNoExistenString}";

                // Muestro ids no encontrados en el mensaje
                ModelState.AddModelError(nameof(libroCreacionDto), mensajeError);

                return ValidationProblem();
            }

            Libro libroDb = await _context.Libros
                .Include(x => x.Autores)
                .FirstAsync(x => x.Id == id);

            if (libroDb is null)
                return NotFound();

            libroDb = _mapper.Map(libroCreacionDto, libroDb);
            AsignarOrdenAutores(libroDb);

            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            int existeLibro = await _context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();

            if (existeLibro == 0)
                return NotFound();

            return NoContent();
        }
        #endregion
    }
}
