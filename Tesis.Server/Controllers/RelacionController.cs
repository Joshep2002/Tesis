using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesis.Application.Services.Evaluacion;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelacionController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IEvaluador _evaluador;
        private readonly ILogger<RelacionController> _logger;

        public RelacionController(IUnitOfWorks unitOfWorks, IEvaluador evaluador, ILogger<RelacionController> logger)
        {
            _unitOfWorks = unitOfWorks;
            _evaluador = evaluador;
            _logger = logger;
        }

        [HttpPatch("indicador-to-proceso/{procesoId}")]
        public async Task<IActionResult> AssignIndicatorsToProcess(int procesoId, [FromBody] List<int> indicadoresList)
        {
            var proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == procesoId, includeProperties: "Indicadores");

            if (proceso == null) return NotFound("El proceso especificado no existe.");

            foreach (var indicadorId in indicadoresList)
            {
                IndicadorModel indicador = await _unitOfWorks.Indicador.Get(i => i.Id == indicadorId);

                if (indicador == null) return NotFound($"El indicador con ID {indicadorId} no existe.");


                // Validar si el indicador ya está asociado a otro proceso

                if (indicador.ProcesoId != null && indicador.ProcesoId != procesoId)
                {
                    return BadRequest($"El indicador con ID {indicadorId} ya está asociado al proceso con ID {indicador.ProcesoId}.");
                }


                // Asignar el procesoId al indicador.
                indicador.ProcesoId = procesoId;


                _unitOfWorks.Indicador.Update(indicador);

            }

            await _unitOfWorks.SaveAsync();
            return Ok("Indicadores asignados al proceso correctamente.");
        }


        [HttpPatch("indicador-to-proceso/deslink/{procesoId}")]
        public async Task<IActionResult> DeslinkIndicadorInProceso(
            int procesoId,
            [FromBody] int indicadorPatch)
        {
            var proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == procesoId, includeProperties: "Indicadores");
            if (proceso == null) return NotFound("El proceso especificado no existe.");

            IndicadorModel indicador = await _unitOfWorks.Indicador.Get(i => i.Id == indicadorPatch);
            if (indicador == null) return NotFound($"El indicador con ID {indicadorPatch} no existe.");

            // Validar si el indicador está vinculado al proceso que se quiere desvincular
            if (indicador.ProcesoId != procesoId)
            {
                return BadRequest($"El indicador {indicadorPatch} no está vinculado al proceso {procesoId}.");
            }

            // Desvincular
            indicador.ProcesoId = null;
            _unitOfWorks.Indicador.Update(indicador);
            await _unitOfWorks.SaveAsync();

            return Ok("Indicador desvinculado correctamente.");
        }


        [HttpPost("vincular-proceso-objetivo")]
        public async Task<IActionResult> VincularProcesoAObjetivo(int procesoId, int objetivoId, List<int> indicadorIdsSeleccionados)
        {
            _logger.LogDebug("Iniciando Vinculacion de Proceso-Indicadores a Objetivo....");

            // Verificar si el objetivo y el proceso existen
            var proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == procesoId, "Indicadores");
            var objetivo = await _unitOfWorks.Objetivo.GetWithIncludes(o => o.Id == objetivoId, "ObjetivoProcesos");

            if (proceso == null)
            {
                _logger.LogWarning("Proceso ID:{procesoId} a Vincular al Objetivo:{idObjetivo} no existe en Base de Datos", procesoId, objetivoId);
                return NotFound("Proceso no encontrado");
            }

            if (objetivo == null)
            {
                _logger.LogWarning("Objetivo:{idObjetivo} no existe en Base de Datos", objetivoId);
                return NotFound("Objetivo no encontrado");
            }

            _logger.LogDebug("Creando relacion entre el proceso - objetivo con los indicadores seleccionados");

            var relacionesAAgregar = new List<ObjetivoProcesoIndicadorModel>();

            foreach (var indicadorId in indicadorIdsSeleccionados)
            {
                var indicador = proceso.Indicadores.FirstOrDefault(i => i.Id == indicadorId);

                if (indicador != null)
                {
                    // Verificar si la relación ya existe
                    var existeRelacion = await _unitOfWorks.ObjetivoProcesoIndicador
                        .GetWithIncludes(opi => opi.ObjetivoId == objetivoId && opi.ProcesoId == procesoId && opi.IndicadorId == indicadorId);

                    if (existeRelacion != null)
                    {
                        // Relación ya existe, no la agregues
                        _logger.LogWarning("La relación Objetivo-{0} Proceso-{1} Indicador-{2} ya existe.", objetivoId, procesoId, indicadorId);
                        return BadRequest($"El indicador {indicadorId} ya esta relacionado al objetivo {objetivoId}");
                    }
                    else
                    {
                        // No existe la relación, puedes agregarla
                        var nuevaRelacion = new ObjetivoProcesoIndicadorModel
                        {
                            ObjetivoId = objetivoId,
                            ProcesoId = procesoId,
                            IndicadorId = indicadorId
                        };

                        relacionesAAgregar.Add(nuevaRelacion);
                        _logger.LogInformation("Nueva Relacion Objetivo-Proceso-Indicador preparada para ser agregada.");
                    }
                }
                else
                {
                    _logger.LogWarning("Indicador ID:{id} no encontrado", indicadorId);
                    return NotFound($"Indicador ID:{indicadorId} no encontrado.");
                }
            }

            // Si hay relaciones a agregar, las agregamos a la base de datos
            if (relacionesAAgregar.Any())
            {
                _unitOfWorks.ObjetivoProcesoIndicador.AddRange(relacionesAAgregar);
                await _unitOfWorks.SaveAsync();
                _logger.LogInformation("Vinculacion Objetivo-Proceso-Indicador realizada correctamente");
            }
            else
            {
                _logger.LogInformation("No se agregaron relaciones, ya existían.");
            }

            return Ok($"Indicadores {string.Join(", ", relacionesAAgregar.Select(r => r.IndicadorId))} agregados al Objetivo {objetivoId}");

        }

        [HttpDelete("desvincular-proceso-objetivo/{objetivoId}/{procesoId}")]
        public async Task<IActionResult> DesvincularProcesoObjetivo(int objetivoId, int procesoId)
        {
            // Buscar todas las relaciones ObjetivoProcesoIndicador que correspondan a ese objetivo y proceso
            var relaciones = await _unitOfWorks.ObjetivoProcesoIndicador.GetAllValuesByParameter(
                opi => opi.ObjetivoId == objetivoId && opi.ProcesoId == procesoId);

            if (!relaciones.Any())
            {
                return NotFound("No se encontraron relaciones para desvincular.");
            }

            _unitOfWorks.ObjetivoProcesoIndicador.RemoveRange(relaciones);
            await _unitOfWorks.SaveAsync();
            return Ok($"El proceso {procesoId} y sus indicadores vinculados fueron desvinculados del objetivo {objetivoId}.");
        }

        [HttpDelete("desvincular-indicadores-proceso")]
        public async Task<IActionResult> DesvincularIndicadoresDeProceso(int procesoId, int objetivoId, [FromBody] List<int> indicadorIds)  
        {
            _logger.LogDebug("Iniciando la desvinculación de indicadores: {IndicadorIds} del proceso {ProcesoId} en el objetivo {ObjetivoId}",
                             string.Join(", ", indicadorIds), procesoId, objetivoId);

            // Obtener las relaciones que coinciden con el objetivo, proceso e indicadores seleccionados.
            var relaciones = await _unitOfWorks.ObjetivoProcesoIndicador.GetAllValuesByParameter(
                opi => opi.ObjetivoId == objetivoId
                       && opi.ProcesoId == procesoId
                       && indicadorIds.Contains(opi.IndicadorId));

            if (!relaciones.Any())
            {
                _logger.LogWarning("No se encontraron relaciones para desvincular para el objetivo {ObjetivoId} y proceso {ProcesoId} con los indicadores especificados.", objetivoId, procesoId);
                return NotFound("No se encontraron relaciones para eliminar.");
            }

            _unitOfWorks.ObjetivoProcesoIndicador.RemoveRange(relaciones);
            await _unitOfWorks.SaveAsync();

            _logger.LogInformation("Se han removido las relaciones de indicadores: {IndicadorIds} del proceso {ProcesoId} en el objetivo {ObjetivoId}.",
                                     string.Join(", ", relaciones.Select(r => r.IndicadorId)), procesoId, objetivoId);

            return Ok($"Se removieron los indicadores {string.Join(", ", relaciones.Select(r => r.IndicadorId))} del proceso {procesoId} en el objetivo {objetivoId}.");
        }

    }

}
