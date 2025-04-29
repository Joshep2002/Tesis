using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.Services
{
    public class EvaluacionService
    {
        public class EvaluationContext
        {
            public int Total { get; init; }
            public int SobreCumplido { get; init; }
            public int Cumplido { get; init; }
            public int ParcialmenteCumplido { get; init; }
            public int Incumplido { get; init; }

            public decimal PctSC => Total > 0 ? SobreCumplido * 100m / Total : 0m;
            public decimal PctC => Total > 0 ? Cumplido * 100m / Total : 0m;
            public decimal PctPC => Total > 0 ? ParcialmenteCumplido * 100m / Total : 0m;
            public decimal PctI => Total > 0 ? Incumplido * 100m / Total : 0m;
        }

        public static class EntityEvaluator
        {
            public static EvaluationStatus Evaluar(IEnumerable<IndicadorModel> indicadores)
            {
                // agrupa por tipo
                var esc = indicadores.Where(i => i.Tipo == IndicadorType.Escencial).ToList();
                var nec = indicadores.Where(i => i.Tipo == IndicadorType.Necesario).ToList();

                var ctxEsc = MakeContext(esc);
                var ctxNec = MakeContext(nec);

                // regla SobreCumplido
                if (ctxEsc.PctSC >= 60 && ctxEsc.PctC <= 40 && ctxEsc.PctPC == 0 && ctxEsc.PctI == 0
                    && ctxNec.PctSC >= 50 && ctxNec.PctC >= 40 && ctxNec.PctPC <= 10 && ctxNec.PctI == 0)
                    return EvaluationStatus.SobreCumplido;

                // regla Cumplido
                if ((ctxEsc.PctSC + ctxEsc.PctC) >= 90 && ctxEsc.PctPC <= 10 && ctxEsc.PctI == 0
                    && (ctxNec.PctSC + ctxNec.PctC) >= 70 && ctxNec.PctPC >= 20 && ctxNec.PctI <= 10)
                    return EvaluationStatus.Cumplido;

                // regla ParcialmenteCumplido
                if ((ctxEsc.PctSC + ctxEsc.PctC + ctxEsc.PctPC) >= 90 && ctxEsc.PctI <= 10
                    && (ctxNec.PctSC + ctxNec.PctC + ctxNec.PctPC) >= 80 && ctxNec.PctI <= 20)
                    return EvaluationStatus.ParcialmenteCumplido;

                return EvaluationStatus.Incumplido;
            }

            private static EvaluationContext MakeContext(IEnumerable<IndicadorModel> items)
            {
                var arr = items.ToArray();
                return new EvaluationContext
                {
                    Total = arr.Length,
                    SobreCumplido = arr.Count(i => i.Evaluacion == EvaluationStatus.SobreCumplido),
                    Cumplido = arr.Count(i => i.Evaluacion == EvaluationStatus.Cumplido),
                    ParcialmenteCumplido = arr.Count(i => i.Evaluacion == EvaluationStatus.ParcialmenteCumplido),
                    Incumplido = arr.Count(i => i.Evaluacion == EvaluationStatus.Incumplido),
                };
            }
        }

    }
}
