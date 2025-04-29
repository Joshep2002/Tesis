using Mapster;
using Microsoft.AspNetCore.Mvc;
using Tesis.Application.DTOs.Indicador;
using Tesis.Application.DTOs.Proceso;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;
using Tesis.Shared;

namespace Tesis.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class IndicadorController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly ILogger<IndicadorController> _logger;

        public IndicadorController(IUnitOfWorks unitOfWorks, ILogger<IndicadorController> logger)
        {
            _unitOfWorks = unitOfWorks;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndicadorModel>>> GetAll()
        {
            _logger.LogDebug("Obteniendo todos los Indicadores");

            var indicadores = await _unitOfWorks.Indicador.GetAllWithIncludes(includeProperties: "Proceso");

            if (!indicadores.Any())
            {
                _logger.LogWarning("No existen indicadores en la base de datos");
                return NotFound("No hay Indicadores creados :)");
            }

            // Adaptamos la colección completa a List<IndicadorDTO> con Mapster
            var indicadoresDTO = indicadores.Adapt<List<IndicadorDTO>>();

            _logger.LogInformation("Se encontraron {indicadorCount} indicadores en la base de datos", indicadores.Count());
            indicadores.AllIndicadoresConsoleMessage();

            return Ok(indicadoresDTO);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<ActionResult<IndicadorModel>> GetIndicadorById(int id)
        {

            _logger.LogDebug("Solicitud recibida en GetById con ID: {@Id} (Tipo: {Type})", id, id.GetType().Name);

            if (id is 0)
            {
                _logger.LogWarning("El ID  es {id} ", id);
                return BadRequest($"El ID debe ser un valor mayor a 0");
            }

            IndicadorModel Indicador = await _unitOfWorks.Indicador.GetWithIncludes(i => i.Id == id, includeProperties: "Proceso");

            if (Indicador is null)
            {
                _logger.LogWarning("Indicador con ID {IndicadorId} no encontrado", id);
                return NotFound($"Indicador con ID :{id} no encontrado");
            }
            // Adaptamos la colección completa a List<IndicadorDTO> con Mapster
            var indicadorDTO = Indicador.Adapt<List<IndicadorDTO>>();

            _logger.LogInformation("Indicador encontrado: {@Indicador}", Indicador);

            return Ok(indicadorDTO);
        }

        [HttpPost("create")]
        public async Task<ActionResult<IndicadorUpsertDto>> Add(IndicadorUpsertDto indicadorDto)
        {
            _logger.LogDebug("Iniciando creación de nuevo indicador");

            if (indicadorDto == null)
            {
                _logger.LogWarning("Intento de crear indicador con DTO nulo");
                return BadRequest("El indicador no puede ser nulo");
            }

            var newIndicador = indicadorDto.Adapt<IndicadorModel>();

            _unitOfWorks.Indicador.Add(newIndicador);
            await _unitOfWorks.SaveAsync();

            _logger.LogInformation("Indicador creado exitosamente. ID: {Id}", newIndicador.Id);

            return CreatedAtAction(
                nameof(GetIndicadorById),
                new { id = newIndicador.Id },
                MapToDto(newIndicador));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateIndicador(int id, IndicadorUpsertDto indicadorDto)
        {
            _logger.LogDebug("Iniciando actualización del indicador {Id}", id);

            // Obtener entidad existente
            var indicadorExistente = await _unitOfWorks.Indicador.GetWithIncludes(p => p.Id == id, includeProperties: "Proceso");

            if (indicadorExistente == null)
            {
                _logger.LogWarning("Indicador {Id} no encontrado", id);
                return NotFound();
            }

            // Mapeo manual DTO -> Model
            indicadorExistente.Descripcion = indicadorDto.Descripcion;
            indicadorExistente.Tipo = indicadorDto.Tipo;
            indicadorExistente.MetaCumplir = indicadorDto.MetaCumplir;
            indicadorExistente.MetaReal = indicadorDto.MetaReal;
            // Persistencia
            _unitOfWorks.Indicador.Update(indicadorExistente);
            await _unitOfWorks.SaveAsync();

            _logger.LogInformation("Indicador {Id} actualizado correctamente", id);
            return NoContent();

        }

        [HttpPatch("update/metaReal/{id}")]
        public async Task<IActionResult> UpdatePatch(int id, string newMetaReal)
        {
            _logger.LogDebug("Actualizando MeteReal: {metaReal} - con ID: {IndicadorId}", newMetaReal, id);

            IndicadorModel Indicador = await _unitOfWorks.Indicador.Get(i => i.Id == id);

            if (Indicador is null)
            {
                _logger.LogWarning("Indicador con ID:{idIndicador} no encontrado", id);
                return NotFound();
            }

            string OldMetaReal = Indicador.MetaReal;
            Indicador.MetaReal = newMetaReal;
            _logger.LogInformation("NewMetaReal:{newMetaReal} asignada a el valor MetaReal:{MetaReal} en el Modelo", newMetaReal, OldMetaReal);

            _unitOfWorks.Indicador.Update(Indicador);
            _unitOfWorks.Save();

            _logger.LogInformation("MetaReal:{metaReal} actualizada en Indicador:{id}", Indicador.MetaReal, Indicador.Id);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteIndicador(int id)
        {
            _logger.LogDebug("Eliminando indicador con ID {IndicadorId}", id);

            IndicadorModel DeleteIndicador = await _unitOfWorks.Indicador.Get(p => p.Id == id);

            if (DeleteIndicador is null)
            {
                _logger.LogWarning("No se encontró el indicador con ID {IndicadorId} para eliminar", id);
                return NotFound("El indicador a eliminar no puede ser nulo");
            }

            _unitOfWorks.Indicador.Remove(DeleteIndicador);
            _unitOfWorks.Save();

            _logger.LogInformation("Indicador eliminado: {@Indicador}", DeleteIndicador);
            return NoContent();
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllIndicadores()
        {
            _logger.LogDebug("Eliminando todos los  indicador de la Base de Datos");

            IEnumerable<IndicadorModel> AllIndicadores = await _unitOfWorks.Indicador.GetAll();

            if (AllIndicadores is null)
            {
                _logger.LogWarning("No hay ningun Indicador en la Base de Datos");
                return NoContent();
            }

            _unitOfWorks.Indicador.RemoveRange(AllIndicadores);
            _unitOfWorks.Save();

            _logger.LogInformation("Todos los Indicadores fueron eliminados de la Base de Datos exitosamente!");

            return NoContent();

        }


        private IndicadorUpsertDto MapToDto(IndicadorModel model)
        {
            return new IndicadorUpsertDto
            {
                Descripcion = model.Descripcion,
                MetaCumplir = model.MetaCumplir,
                MetaReal = model.MetaReal,
                Tipo = model.Tipo
            };
        }
    }
}