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
                .Include(libro => libro.Autor)
                .ToListAsync();
            IEnumerable<LibroDto> librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return librosDto;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDto>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(x => x.Autor)
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
            Libro libro = _mapper.Map<Libro>(libroCreacionDto);
            bool existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"No existe el autor {libro.AutorId}");
                return ValidationProblem();
            }

            _context.Add(libro);
            await _context.SaveChangesAsync();
            LibroDto libroDto = _mapper.Map<LibroDto>(libro);
            return CreatedAtRoute("ObtenerLibro", new {id = libro.Id}, libroDto);
        }
        #endregion

        #region Put
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDto libroCreacionDto)
        {
            Libro libro = _mapper.Map<Libro>(libroCreacionDto);
            libro.Id = id; // Importante

            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!existeLibro)
                return NotFound();

            bool existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
                return BadRequest($"No existe el autor {libro.Id}");

            _context.Update(libro);
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
