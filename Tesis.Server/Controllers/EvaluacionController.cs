using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tesis.Application.Services.Evaluacion;
using Tesis.DataAcces.Repository.IRepository;

namespace Tesis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionController : ControllerBase
    {
        private readonly IEvaluador _evaluador;

        public EvaluacionController(IEvaluador evaluador)
            => _evaluador = evaluador;

        [HttpPost("evaluar-proceso/{id}")]
        public async Task<IActionResult> EvaluateProceso(int id)
        {

            var result = await _evaluador.EvaluarProcesoAsync(id);
            if (result is null) return NoContent();
            return Ok(result);
        }

        [HttpPost("evaluar-objetivo/{id}")]
        public async Task<IActionResult> EvaluateObjetivo(int id)
        {
            var result = await _evaluador.EvaluarObjetivoAsync(id);
            if (result is null) return NoContent();
            return Ok(result);
        }

        [HttpPost("evaluar-procesos")]
        public async Task<IActionResult> EvaluateAllProcesos()
        {
            var procesos = await _evaluador.EvaluarTodosProcesosAsync();

            if (!procesos.Any())
                return NoContent();

            return Ok(procesos);
        }

        [HttpPost("evaluar-objetivos")]
        public async Task<IActionResult> EvaluateAllObjetivos()
        {
            var objetivos = await _evaluador.EvaluarTodosObjetivosAsync();

            if (!objetivos.Any())
                return NoContent();

            return Ok(objetivos);
        }
    }


    /* 
     Flujo en la vista Blazor
     Botón individual de "Evaluar":

     Llama al endpoint /api/Proceso/evaluate/{id}.

     Actualiza el estado del proceso específico en la tabla.

     Botón de "Evaluar Todos":

     Llama al endpoint /api/Proceso/evaluate-all.

     Actualiza el estado de todos los procesos en la tabla.
     */
}

