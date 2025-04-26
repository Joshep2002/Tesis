using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.Services.Evaluacion
{
    public interface IEvaluador
    {
        EvaluationStatus EvaluarPorIndicadores(IEnumerable<IndicadorModel> indicadores);
    }
}
