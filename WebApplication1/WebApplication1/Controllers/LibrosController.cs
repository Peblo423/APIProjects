using AutoMapper;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<LibroDto>> Get()
        {
            var libros = await _context.Libros
                .Include(libro => libro.Autores)
                .ToListAsync();
            IEnumerable<LibroDto> librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return librosDto;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDto>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(x => x.Autores)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound();

            }
            LibroDto libroDto = _mapper.Map<LibroDto>(libro);
            return libroDto;
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
        private void AsignarOrdenAutores(Libro libro)
        {
            if(libro is not null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i;
                }
            }
        }
        #endregion

        //#region Put
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> Put(int id, LibroCreacionDto libroCreacionDto)
        //{
        //    Libro libro = _mapper.Map<Libro>(libroCreacionDto);
        //    libro.Id = id; // Importante

        //    bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == id);

        //    if (!existeLibro)
        //        return NotFound();

        //    bool existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

        //    if (!existeAutor)
        //        return BadRequest($"No existe el autor {libro.Id}");

        //    _context.Update(libro);
        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}
        //#endregion

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
