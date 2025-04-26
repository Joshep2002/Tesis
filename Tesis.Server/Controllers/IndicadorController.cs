using Microsoft.AspNetCore.Mvc;
using Serilog;
using Tesis.Application.DTOs.Indicador;
using Tesis.Application.Services.IndicadorServices;
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

            // Mapear a DTOs
            var indicadoresDTO = indicadores.Select(indicador => new IndicadorDTO
            {
                Id = indicador.Id,
                Descripcion = indicador.Descripcion,
                MetaCumplir = indicador.MetaCumplir,
                MetaReal = indicador.MetaReal,
                Evaluacion = indicador.Evaluacion,
                Tipo = indicador.Tipo
            }).ToList();

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

            IndicadorModel Indicador = await _unitOfWorks.Indicador.Get(i => i.Id == id);

            if (Indicador is null)
            {
                _logger.LogWarning("Indicador con ID {IndicadorId} no encontrado", id);
                return NotFound($"Indicador con ID :{id} no encontrado");
            }
            var indicadorDto = new IndicadorDTO
            {
                Id = Indicador.Id,
                Descripcion = Indicador.Descripcion,
                MetaCumplir = Indicador.MetaCumplir,
                MetaReal = Indicador.MetaReal,
                Evaluacion = Indicador.Evaluacion,
                Tipo = Indicador.Tipo
            };

            _logger.LogInformation("Indicador encontrado: {@Indicador}", Indicador);

            return Ok(indicadorDto);
        }

        [HttpPost("create")]
        public async Task<ActionResult<IndicadorUpsertDto>> Add(IndicadorUpsertDto indicadorDto)
        {
            _logger.LogDebug("Iniciando creación de nuevo indicador");

            try
            {
                
                if (indicadorDto == null)
                {
                    _logger.LogWarning("Intento de crear indicador con DTO nulo");
                    return BadRequest("El indicador no puede ser nulo");
                }

               
                var newIndicador = new IndicadorModel
                {
                    Descripcion = indicadorDto.Descripcion,
                    MetaCumplir = indicadorDto.MetaCumplir,
                    MetaReal = indicadorDto.MetaReal,
                    Tipo = indicadorDto.Tipo
                };

              
                if (!newIndicador.TryConvertMetaCumplir(out string conversionError))
                {
                    _logger.LogWarning("Error en conversión de MetaCumplir: {Error}", conversionError);
                    return BadRequest(conversionError);
                }

              
                if (!string.IsNullOrEmpty(newIndicador.MetaReal))
                {
                    newIndicador.NormalizarMetaReal();

                    if (!IndicadorEvaluator.EvaluarIndicador(newIndicador, out string evalError))
                    {
                        _logger.LogWarning("Error en evaluación: {Error}", evalError);
                        return BadRequest(evalError);
                    }
                }

              
                _unitOfWorks.Indicador.Add(newIndicador);
                await _unitOfWorks.SaveAsync();

                _logger.LogInformation("Indicador creado exitosamente. ID: {Id}", newIndicador.Id);

               
                return CreatedAtAction(
                    nameof(GetIndicadorById),
                    new { id = newIndicador.Id },
                    MapToDto(newIndicador));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico creando indicador");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateIndicador(int id, IndicadorUpsertDto indicadorDto)
        {
            _logger.LogDebug("Iniciando actualización del indicador {Id}", id);

            // Validación del modelo usando data annotations
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido para el indicador {Id}. Errores: {@Errores}",
                    id, ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest(ModelState);
            }

            try
            {
                // Obtener entidad existente
                var indicadorExistente = await _unitOfWorks.Indicador.Get(p => p.Id == id);

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

                // Lógica de negocio
                if (!indicadorExistente.TryConvertMetaCumplir(out string conversionError))
                {
                    _logger.LogError("Error conversión MetaCumplir: {Error}", conversionError);
                    return BadRequest(conversionError);
                }

                if (!string.IsNullOrEmpty(indicadorExistente.MetaReal))
                {
                    indicadorExistente.NormalizarMetaReal();

                    if (!IndicadorEvaluator.EvaluarIndicador(indicadorExistente, out string evalError))
                    {
                        _logger.LogError("Error evaluación: {Error}", evalError);
                        return BadRequest(evalError);
                    }
                }

                // Persistencia
                _unitOfWorks.Indicador.Update(indicadorExistente);
                await _unitOfWorks.SaveAsync();

                _logger.LogInformation("Indicador {Id} actualizado correctamente", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error crítico actualizando indicador {Id}", id);
                return StatusCode(500, new { Error = "Error interno del servidor", Detalle = ex.Message });
            }
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

            // Normalizamos el valor de MetaReal:
            Indicador.NormalizarMetaReal();
            _logger.LogInformation("Valor MetaReal ha sido 'Normalizada'");

            // Llamar a la lógica de evaluación:
            if (!IndicadorEvaluator.EvaluarIndicador(Indicador, out string error))
            {
                return BadRequest(error);
            }

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