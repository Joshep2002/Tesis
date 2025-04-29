using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesis.Application.DTOs.Indicador;
using Tesis.Application.DTOs.Objetivo;
using Tesis.Application.DTOs.Proceso;
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
        public async Task<ActionResult<IEnumerable<ObjetivoDTO>>> GetAll()
        {
            var objetivos = await _unitOfWorks.Objetivo.GetAllWithIncludes("ObjetivoProcesosIndicadores.Proceso,ObjetivoProcesosIndicadores.Indicador");

            if (!objetivos.Any()) return NotFound("No hay objetivos");

            // Mapear primero el objetivo a ObjetivoResponseDTO
            var objetivosDto = objetivos.Adapt<List<ObjetivoDTO>>();

            return Ok(objetivosDto);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObjetivoDTO>> GetObjetivoById(int id)
        {
            var objetivo = await _unitOfWorks.Objetivo.GetWithIncludes(
         o => o.Id == id,
         "ObjetivoProcesosIndicadores.Proceso,ObjetivoProcesosIndicadores.Indicador");

            if (objetivo is null) return NotFound($"El Objetivo con id: {id} no existe");

            var objetivoDto = objetivo.Adapt<ObjetivoDTO>();

            return Ok(objetivoDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Add(UpsertObjetivoDTO newObjetivoDto)
        {
            if (newObjetivoDto is null)
                return BadRequest("El nuevo Objetivo no puede ser nulo");

            ObjetivoModel Objetivo = new ObjetivoModel
            {
                Nombre = newObjetivoDto.Nombre,

            };

            _unitOfWorks.Objetivo.Add(Objetivo);
            await _unitOfWorks.SaveAsync();

            return CreatedAtAction(nameof(GetObjetivoById), new { id = Objetivo.Id }, Objetivo);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, UpsertObjetivoDTO updateDto)
        {
            ObjetivoModel Objetivo = await _unitOfWorks.Objetivo.Get(o => o.Id == id);

            if (Objetivo is null) return NotFound("El Objetivo no existe");

            Objetivo.Nombre = updateDto.Nombre;

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
