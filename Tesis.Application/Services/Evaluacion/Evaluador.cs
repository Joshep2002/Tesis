using Tesis.Application.Services.Extensions;
using Tesis.DataAcces.Repository.IRepository;
using Tesis.Domain.Models;

namespace Tesis.Application.Services.Evaluacion
{
    public class Evaluador : IEvaluador
    {

        private readonly IUnitOfWorks _unitOfWorks;

        public Evaluador(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task<ProcesoModel> EvaluarProcesoAsync(int procesoId)
        {
            var proceso = await _unitOfWorks.Proceso
                .GetWithIncludes(
                    p => p.Id == procesoId,
                    includeProperties: "Indicadores");

            if (proceso is null)
                return null;

            if (proceso.Indicadores == null || !proceso.Indicadores.Any())
            {
                return null; // Si no tiene indicadores, no lo evaluamos, devolvemos null
            }

            proceso.Reevaluar();
            _unitOfWorks.Proceso.Update(proceso);
            await _unitOfWorks.SaveAsync();
            return proceso;
        }

        public async Task<ObjetivoModel> EvaluarObjetivoAsync(int objetivoId)
        {
            // Incluimos la relación de la entidad de unión para cargar sus indicadores.
            var objetivo = await _unitOfWorks.Objetivo
                .GetWithIncludes(
                    o => o.Id == objetivoId,
                    includeProperties: "ObjetivoProcesosIndicadores.Indicador");

            if (objetivo is null)
                return null;

            // Extraemos los indicadores a partir de la nueva relación join.
            var indicadoresObjetivo = objetivo.ObjetivoProcesosIndicadores
                .Select(opi => opi.Indicador)
                .Where(i => i != null)
                .Distinct() // Opción: DistinctBy(i => i.Id) si usas System.Linq.Dynamic o similar.
                .ToList();

            if (indicadoresObjetivo == null || !indicadoresObjetivo.Any())
            {
                return null; // Si no tiene indicadores, no lo evaluamos, devolvemos null
            }


            objetivo.Reevaluar();               // <-- tu extensión para Objetivo
             _unitOfWorks.Objetivo.Update(objetivo);
             _unitOfWorks.Save();
            return objetivo;
        }

        public async Task<IEnumerable<ProcesoModel>> EvaluarTodosProcesosAsync()
        {
            var todos = await _unitOfWorks.Proceso
                .GetAllWithIncludes(
                   includeProperties: "Indicadores");

            var procesosConIndicadores = todos.Where(p => p.Indicadores != null && p.Indicadores.Any()).ToList();

            if (!procesosConIndicadores.Any())
            {
                return Enumerable.Empty<ProcesoModel>(); // Si no hay procesos con indicadores, devolver una lista vacía
            }

            foreach (var p in todos)
            {
                p.Reevaluar();
                _unitOfWorks.Proceso.Update(p);
            }
            await _unitOfWorks.SaveAsync();
            return todos;
        }

        public async Task<IEnumerable<ObjetivoModel>> EvaluarTodosObjetivosAsync()
        {
            var todos = await _unitOfWorks.Objetivo
                .GetAllWithIncludes(
                   includeProperties: "ObjetivoProcesos.Proceso.Indicadores,ObjetivoIndicadores.Indicador");

            var objetivosConIndicadores = todos
        .Where(o => o.ObjetivoProcesos
                    .Any(op => op.Proceso.Indicadores != null && op.Proceso.Indicadores.Any()))
        .ToList();

            if (!objetivosConIndicadores.Any())
            {
                return Enumerable.Empty<ObjetivoModel>(); // Si no hay objetivos con indicadores, devolver una lista vacía
            }

            foreach (var o in todos)
            {
                o.Reevaluar();
                _unitOfWorks.Objetivo.Update(o);
            }
            await _unitOfWorks.SaveAsync();
            return todos;
        }
    }
}

/* 
 Mejoras futuras: Puedes encapsular los umbrales en constantes o incluso permitir configurarlos de forma externa (por ejemplo, en un archivo de configuración o en una base de datos) para mayor flexibilidad.
 */