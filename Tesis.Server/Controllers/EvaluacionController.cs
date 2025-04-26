using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesis.Application.Services.Evaluacion;
using Tesis.DataAcces.Repository.IRepository;

namespace Tesis.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionController : ControllerBase
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IEvaluador _evaluador;

        public EvaluacionController(IUnitOfWorks unitOfWorks, IEvaluador evaluador)
        {
            _unitOfWorks = unitOfWorks;
            _evaluador = evaluador;
        }

        [HttpPost("evaluar-proceso/{id}")]
        public async Task<IActionResult> EvaluateProceso(int id)
        {
            var proceso = await _unitOfWorks.Proceso.GetWithIncludes(p => p.Id == id, includeProperties: "Indicadores");

            if (proceso == null) return NotFound();

            _evaluador.EvaluarPorIndicadores(proceso.Indicadores);

            _unitOfWorks.Proceso.Update(proceso);
            await _unitOfWorks.SaveAsync();

            return Ok(proceso);
        }

        [HttpPost("evaluar-objetivo/{id}")]
        public async Task<IActionResult> EvaluateObjetivo(int id)
        {
            var objetivo = await _unitOfWorks.Objetivo.GetWithIncludes(p => p.Id == id, includeProperties: "ObjetivoIndicadores");

            if (objetivo == null) return NotFound();

            var indicadoresObjetivo = objetivo.ObjetivoProcesos.SelectMany(op => op.Proceso.Indicadores);
            objetivo.Evaluacion = _evaluador.EvaluarPorIndicadores(indicadoresObjetivo);

            _unitOfWorks.Objetivo.Update(objetivo);
            _unitOfWorks.Save();

            return Ok(objetivo);
        }



        [HttpPost("evaluar-all")]
        public async Task<IActionResult> EvaluateAll()
        {
            var procesos = await _unitOfWorks.Proceso.GetAll();

            foreach (var proceso in procesos)
            {
                _evaluador.EvaluarPorIndicadores(proceso.Indicadores);
                _unitOfWorks.Proceso.Update(proceso);
            }

            await _unitOfWorks.SaveAsync();

            return Ok(procesos);
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
}
