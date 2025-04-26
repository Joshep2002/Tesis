using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjetivoController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWorks;
        public ObjetivoController(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ObjetivoModel>>> GetAll()
        {
            IEnumerable<ObjetivoModel> Objetivos = await _unitOfWorks.Objetivo.GetAll();

            if (Objetivos is null) return NotFound("No hay objetivos");

            return Ok(Objetivos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObjetivoModel>> GetObjetivoById(int id)
        {
            ObjetivoModel Objetivo = await _unitOfWorks.Objetivo.GetWithIncludes(o => o.Id == id, includeProperties: "ObjetivoIndicadores");

            if (Objetivo is null) return NotFound($"El Objetivo con id: {id} no existe");

            return Ok(Objetivo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Add(ObjetivoModel newObjetivo)
        {
            if (newObjetivo is null)
                return BadRequest("El nuevo Objetivo no puede ser nulo");

            _unitOfWorks.Objetivo.Add(newObjetivo);
            await _unitOfWorks.SaveAsync();

            return CreatedAtAction(nameof(GetObjetivoById), new { id = newObjetivo.Id }, newObjetivo);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, ObjetivoModel updateObjetivo)
        {
            ObjetivoModel Objetivo = await _unitOfWorks.Objetivo.Get(o => o.Id == id);

            if (Objetivo is null) return NotFound("El Objetivo no existe");

            Objetivo.Nombre = updateObjetivo.Nombre;

            _unitOfWorks.Objetivo.Update(Objetivo);
            _unitOfWorks.Save();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteObjetivo(int id)
        {
            ObjetivoModel Objetivo = await _unitOfWorks.Objetivo.Get(o => o.Id == id);

            if (Objetivo is null) return NotFound("El Objetivo no existe");

            _unitOfWorks.Objetivo.Remove(Objetivo);
            _unitOfWorks.Save();

            return NoContent();
        }
    }
}
