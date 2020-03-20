using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiModulo5.Contexts;
using WebApiModulo5.Dtos;
using WebApiModulo5.Models;

namespace WebApiModulo5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDto>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            var autoresDto = mapper.Map<List<AutorDto>>(autores);

            return autoresDto;
        }

        [HttpGet("{id}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDto>> Get(int id)
        {
            var autor = await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            var autorDto = mapper.Map<AutorDto>(autor);
            return autorDto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDto autorCreacion)
        {
            //context.Autores.Add(autor);
            var autor = mapper.Map<Autor>(autorCreacion);
            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDto = mapper.Map<AutorDto>(autor);
            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, autorDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] AutorCreacionDto value)
        {

            var autor = mapper.Map<Autor>(value);
            autor.Id = id;
            context.Entry(autor).State = EntityState.Modified;
            await context.SaveChangesAsync();
            //return Ok();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<Autor> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var autorDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorDB == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(autorDB, ModelState);

            var isValid = TryValidateModel(autorDB);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Autor> Delete(int id)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            context.Autores.Remove(autor);
            context.SaveChanges();
            return autor;
        }


    }
}
