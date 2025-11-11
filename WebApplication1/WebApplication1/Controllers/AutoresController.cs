using AutoMapper;
using Azure;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region GET
        [HttpGet]
        public async Task<IEnumerable<AutorDto>> Get()
        {
            var autores = await _context.Autores.ToListAsync();
            IEnumerable<AutorDto> autoresDto = _mapper.Map<IEnumerable<AutorDto>>(autores);
            return autoresDto;
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDto>> Get([FromRoute] int id)
        {
            var autor = await _context.Autores
                .Include(x => x.Libros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            AutorDto autorDto = _mapper.Map<AutorDto>(autor);
            return autorDto;
        }
        #endregion

        #region Post
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDto autorCreacionDto)
        {
            var autor = _mapper.Map<Autor>(autorCreacionDto);
            _context.Add(autor);
            await _context.SaveChangesAsync();
            AutorDto autorDto = _mapper.Map<AutorDto>(autor);
            return CreatedAtRoute("ObtenerAutor", new {id = autor.Id}, autor);
        }
        #endregion

        #region Put
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, AutorCreacionDto AutorCreacionDto)
        {

            var autor = _mapper.Map<Autor>(AutorCreacionDto);
            autor.Id = id; // Importante
            _context.Update(autor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Patch
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<AutorPatchDto> patchDoc)
        {
            if(patchDoc is null)
            {
                return BadRequest();
            }

            Autor? autorDb = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if(autorDb is null)
            {
                return NotFound();
            }

            // Me paso el modelo a un DTO para manipular una copia y no el original
            AutorPatchDto autorPatchDto = _mapper.Map<AutorPatchDto>(autorDb);

            // El JsonPatchDocument me permite aplicar el parche al autor
            patchDoc.ApplyTo(autorPatchDto, ModelState);

            // Se valida con un método del ControllerBase
            bool esValido = TryValidateModel(autorPatchDto);
            if(!esValido)
            {
                return ValidationProblem();
            }

            // Recién ahora pasamos el Dto a un Autor de Modelo
            _mapper.Map(autorPatchDto, autorDb);

            // No hace falta hacer _context.Update(autorDb)
            // porque este autor ya vino del contexto
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Delete
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            int registrosBorrados = await _context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
        #endregion
    }
}
