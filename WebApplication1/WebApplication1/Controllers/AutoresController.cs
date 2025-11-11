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
            autor.Id = id;
            _context.Update(autor);
            await _context.SaveChangesAsync();
            return Ok();
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
            return Ok();
        }
        #endregion
    }
}
