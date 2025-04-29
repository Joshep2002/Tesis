using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tesis.Application.Services.EvaluacionService;
using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.Services.Extensions
{
    public static class Extensions
    {
        public static void Reevaluar(this ProcesoModel proceso)
        {
            proceso.Evaluacion =
                EntityEvaluator.Evaluar(proceso.Indicadores);
        }

        public static void Reevaluar(this ObjetivoModel objetivo)
        {
            // Extrae los indicadores de la relación join
            var indicadores = objetivo.ObjetivoProcesosIndicadores?
                .Select(opi => opi.Indicador)
                .Where(i => i != null)
                .ToList();

            if (indicadores == null || !indicadores.Any())
            {
                objetivo.Evaluacion = EvaluationStatus.NoEvaluado;
                return;
            }

            // Calcula la evaluación según tu lógica de negocio
            objetivo.Evaluacion = EntityEvaluator.Evaluar(indicadores);
        }
    }

}
