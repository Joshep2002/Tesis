using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.Services.Evaluacion
{
    public interface IEvaluador
    {
        Task<ProcesoModel> EvaluarProcesoAsync(int procesoId);
        Task<ObjetivoModel> EvaluarObjetivoAsync(int objetivoId);
        Task<IEnumerable<ProcesoModel>> EvaluarTodosProcesosAsync();
        Task<IEnumerable<ObjetivoModel>> EvaluarTodosObjetivosAsync();
    }
}
