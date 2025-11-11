using BibliotecaAPI.Data;
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

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get
        [HttpGet]
        public async Task<IEnumerable<Libro>> Get()
        {
            return await _context.Libros.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro = await _context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound();

            }
            return libro;
        }
        #endregion

        #region Post
        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            bool existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"No existe el autor {libro.AutorId}");
                return ValidationProblem();
            }
                
            _context.Add(libro);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("ObtenerLibro", new {id = libro.Id}, libro);
        }
        #endregion

        #region Put
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Libro libro)
        {
            if (id != libro.Id)
                return BadRequest("Los id deben coincidir");

            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == id);

            if (!existeLibro)
                return NotFound();

            bool existeAutor = await _context.Autores.AnyAsync(x => x.Id == libro.AutorId);

            if (!existeAutor)
                return BadRequest($"No existe el autor {libro.Id}");

            _context.Update(libro);
            await _context.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            int existeLibro = await _context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();

            if (existeLibro == 0)
                return NotFound();

            return Ok();
        }
        #endregion
    }
}
