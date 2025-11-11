using AutoMapper;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Get

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDto>>> Get(int libroId)
        {
            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if(!existeLibro)
            {
                return NotFound();
            }

            List<Comentario> comentarios = await _context.Comentarios
                .Where(x => x.LibroId == libroId)
                .OrderByDescending(x => x.FechaPublicacion)
                .ToListAsync();

            return _mapper.Map<List<ComentarioDto>>(comentarios);
        }

        [HttpGet("{id}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDto>> Get(Guid id)
        {
            Comentario? comentario = await _context.Comentarios
                .FirstOrDefaultAsync(x => x.Id == id);
            if(comentario is null)
            {
                return NotFound();
            }

            return _mapper.Map<ComentarioDto>(comentario);
        }
        #endregion

        #region Post

        [HttpPost]

        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDto comentarioCreacionDto)
        {
            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if(!existeLibro)
            {
                return NotFound();
            }

            Comentario comentario = _mapper.Map<Comentario>(comentarioCreacionDto);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;

            _context.Add(comentario);
            await _context.SaveChangesAsync();

            ComentarioDto comentarioDto = _mapper.Map<ComentarioDto>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDto);
        }

        #endregion

        #region Patch
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch([FromRoute] Guid id, int libroId ,[FromBody] JsonPatchDocument<ComentarioPatchDto> patchDoc)
        {
            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            if (patchDoc is null)
            {
                return BadRequest();
            }

            Comentario? comentarioDb = await _context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentarioDb is null)
            {
                return NotFound();
            }

            ComentarioPatchDto comentarioPatchDto = _mapper.Map<ComentarioPatchDto>(comentarioDb);

            patchDoc.ApplyTo(comentarioPatchDto, ModelState);

            bool esValido = TryValidateModel(comentarioPatchDto);
            if (!esValido)
            {
                return ValidationProblem();
            }

            _mapper.Map(comentarioPatchDto, comentarioDb);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Delete
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            bool existeLibro = await _context.Libros.AnyAsync(x => x.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            int registrosBorrados = await _context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();
            if(registrosBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
        #endregion
    }
}
