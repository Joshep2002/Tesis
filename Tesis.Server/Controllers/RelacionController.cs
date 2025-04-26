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

        public RelacionController(IUnitOfWorks unitOfWorks, IEvaluador evaluador)
        {
            _unitOfWorks = unitOfWorks;
            _evaluador = evaluador;
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



        // Ejemplo de endpoint en el controlador
        [HttpPost("objetivos/{objectiveId}/indicador")]
        public async Task<IActionResult> LinkIndicatorsToObjective(int objectiveId, [FromBody] List<int> indicatorIds)
        {
            // Obtener el objetivo; es recomendable incluir las relaciones actuales para evitar duplicados.
            var objective = await _unitOfWorks.Objetivo.GetWithIncludes(
                                 o => o.Id == objectiveId,
                                 includeProperties: "ObjetivoIndicadores");

            if (objective == null)
                return NotFound($"El objetivo con ID {objectiveId} no existe.");

            // Iterar por cada ID de indicador a vincular
            foreach (var indicatorId in indicatorIds)
            {
                // Buscar el indicador para validar su existencia
                var indicador = await _unitOfWorks.Indicador.Get(i => i.Id == indicatorId);
                if (indicador == null)
                    return NotFound($"El indicador con ID {indicatorId} no existe.");

                // Verificar si ya existe la relación (esto asume que en ObjetivoIndicadores existe la propiedad IndicadorId)
                bool existeRelacion = objective.ObjetivoIndicadores
                                                 .Any(oi => oi.IndicadorId == indicatorId);

                if (!existeRelacion)
                {
                    // Crear la relación. Si en tu modelo se manejan propiedades adicionales (por ejemplo, fecha de vinculación),
                    // agrégalas aquí según convenga.
                    var objetivoIndicador = new ObjetivoIndicadorModel
                    {
                        ObjetivoId = objective.Id,
                        IndicadorId = indicatorId
                    };

                    // Agregar la relación al objetivo.
                    objective.ObjetivoIndicadores.Add(objetivoIndicador);
                }
            }

            // Persistir cambios en la base de datos
            await _unitOfWorks.SaveAsync();

            return Ok("Indicadores vinculados al objetivo exitosamente.");
        }
    }
}
