using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.Services.Evaluacion
{
    public class Evaluador :IEvaluador
    {
        public  EvaluationStatus EvaluarPorIndicadores(IEnumerable<IndicadorModel> indicadores)
        {
            // Separamos los indicadores en esenciales y necesarios.
            var escenciales = indicadores.Where(i => i.Tipo == IndicadorType.Escencial).ToList();
            var necesarios = indicadores.Where(i => i.Tipo == IndicadorType.Necesario).ToList();

            // Conteos.
            decimal totalEscenciales = escenciales.Count;
            decimal totalNecesarios = necesarios.Count;

            // Obtenemos los diccionarios para cada grupo (aquí puedes aplicar el mapeo de NoEvaluado a Incumplido si lo requerís)
            var escencialesPorEvaluacion = ClasificarIndicadores(escenciales);
            var necesariosPorEvaluacion = ClasificarIndicadores(necesarios);

            // Caso 1: Solo hay indicadores esenciales.
            if (totalEscenciales > 0 && totalNecesarios == 0)
            {
                decimal porcSobreE = (totalEscenciales > 0 ? (escencialesPorEvaluacion.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) / totalEscenciales) * 100 : 0);
                decimal porcCumplidoE = (totalEscenciales > 0 ? (escencialesPorEvaluacion.GetValueOrDefault(EvaluationStatus.Cumplido, 0) / totalEscenciales) * 100 : 0);
                decimal porcParcialE = (totalEscenciales > 0 ? (escencialesPorEvaluacion.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0) / totalEscenciales) * 100 : 0);
                decimal porcIncumplidoE = (totalEscenciales > 0 ? (escencialesPorEvaluacion.GetValueOrDefault(EvaluationStatus.Incumplido, 0) / totalEscenciales) * 100 : 0);

                if (porcSobreE >= 60 && porcCumplidoE <= 40 && porcParcialE == 0 && porcIncumplidoE == 0)
                    return EvaluationStatus.SobreCumplido;
                else if ((porcSobreE + porcCumplidoE) >= 90 && porcParcialE <= 10 && porcIncumplidoE == 0)
                    return EvaluationStatus.Cumplido;
                else if ((porcSobreE + porcCumplidoE + porcParcialE) >= 90 && porcIncumplidoE <= 10)
                    return EvaluationStatus.ParcialmenteCumplido;
                else
                    return EvaluationStatus.Incumplido;
            }
            // Caso 2: Solo hay indicadores necesarios.
            else if (totalNecesarios > 0 && totalEscenciales == 0)
            {
                decimal porcSobreN = (totalNecesarios > 0 ? (necesariosPorEvaluacion.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) / totalNecesarios) * 100 : 0);
                decimal porcCumplidoN = (totalNecesarios > 0 ? (necesariosPorEvaluacion.GetValueOrDefault(EvaluationStatus.Cumplido, 0) / totalNecesarios) * 100 : 0);
                decimal porcParcialN = (totalNecesarios > 0 ? (necesariosPorEvaluacion.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0) / totalNecesarios) * 100 : 0);
                decimal porcIncumplidoN = (totalNecesarios > 0 ? (necesariosPorEvaluacion.GetValueOrDefault(EvaluationStatus.Incumplido, 0) / totalNecesarios) * 100 : 0);

                if (porcSobreN >= 50 && porcCumplidoN >= 40 && porcParcialN <= 10 && porcIncumplidoN == 0)
                    return EvaluationStatus.SobreCumplido;
                else if ((porcSobreN + porcCumplidoN) >= 70 && porcParcialN >= 20 && porcIncumplidoN <= 10)
                    return EvaluationStatus.Cumplido;
                else if ((porcSobreN + porcCumplidoN + porcParcialN) >= 80 && porcIncumplidoN <= 20)
                    return EvaluationStatus.ParcialmenteCumplido;
                else
                    return EvaluationStatus.Incumplido;
            }
            // Caso 3: Existen tanto indicadores esenciales como necesarios.
            else if (totalEscenciales > 0 && totalNecesarios > 0)
            {
                // Puedes reutilizar la lógica actual para el caso combinado.
                if (EsSobrecumplido(escencialesPorEvaluacion, necesariosPorEvaluacion, totalEscenciales, totalNecesarios))
                    return EvaluationStatus.SobreCumplido;
                else if (EsCumplido(escencialesPorEvaluacion, necesariosPorEvaluacion, totalEscenciales, totalNecesarios))
                    return EvaluationStatus.Cumplido;
                else if (EsParcialmenteCumplido(escencialesPorEvaluacion, necesariosPorEvaluacion, totalEscenciales, totalNecesarios))
                    return EvaluationStatus.ParcialmenteCumplido;
                else
                    return EvaluationStatus.Incumplido;
            }
            // Si no hay indicadores en absoluto, asignamos un estado predeterminado.
            return EvaluationStatus.NoEvaluado;
        }


        // Los métodos EsSobrecumplido, EsCumplido y EsParcialmenteCumplido se mantienen para el caso combinado.
        private bool EsSobrecumplido(Dictionary<EvaluationStatus, int> escenciales, Dictionary<EvaluationStatus, int> necesarios, decimal totalEscenciales, decimal totalNecesarios)
        {
            // Aquí usas la lógica que ya tenías para el caso combinado. Por ejemplo:
            if (totalEscenciales == 0 || totalNecesarios == 0) return false;
            decimal sobrecumplidosEscenciales = (escenciales.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) / totalEscenciales) * 100;
            decimal cumplidosEscenciales = (escenciales.GetValueOrDefault(EvaluationStatus.Cumplido, 0) / totalEscenciales) * 100;
            decimal sobrecumplidosNecesarios = (necesarios.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) / totalNecesarios) * 100;
            decimal cumplidosNecesarios = (necesarios.GetValueOrDefault(EvaluationStatus.Cumplido, 0) / totalNecesarios) * 100;
            decimal parcialmenteNecesarios = (necesarios.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0) / totalNecesarios) * 100;

            return sobrecumplidosEscenciales >= 60 &&
                   cumplidosEscenciales <= 40 &&
                   sobrecumplidosNecesarios >= 50 &&
                   cumplidosNecesarios >= 40 &&
                   parcialmenteNecesarios <= 10;
        }

        private bool EsCumplido(Dictionary<EvaluationStatus, int> escenciales, Dictionary<EvaluationStatus, int> necesarios, decimal totalEscenciales, decimal totalNecesarios)
        {
            if (totalEscenciales == 0 || totalNecesarios == 0) return false;
            decimal escencialesSum = ((escenciales.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) + escenciales.GetValueOrDefault(EvaluationStatus.Cumplido, 0)) / totalEscenciales) * 100;
            decimal parcialEscenciales = (escenciales.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0) / totalEscenciales) * 100;

            decimal necesariosSum = ((necesarios.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0) + necesarios.GetValueOrDefault(EvaluationStatus.Cumplido, 0)) / totalNecesarios) * 100;
            decimal parcialNecesarios = (necesarios.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0) / totalNecesarios) * 100;

            return escencialesSum >= 90 && parcialEscenciales <= 10 &&
                   necesariosSum >= 70 && parcialNecesarios >= 20; // Ajusta este umbral según tus reglas
        }

        private bool EsParcialmenteCumplido(Dictionary<EvaluationStatus, int> escenciales, Dictionary<EvaluationStatus, int> necesarios, decimal totalEscenciales, decimal totalNecesarios)
        {
            if (totalEscenciales == 0 || totalNecesarios == 0) return false;
            decimal escencialesSum = ((escenciales.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0)
                                        + escenciales.GetValueOrDefault(EvaluationStatus.Cumplido, 0)
                                        + escenciales.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0)) / totalEscenciales) * 100;
            decimal incumplidosEscenciales = (escenciales.GetValueOrDefault(EvaluationStatus.Incumplido, 0) / totalEscenciales) * 100;

            decimal necesariosSum = ((necesarios.GetValueOrDefault(EvaluationStatus.SobreCumplido, 0)
                                       + necesarios.GetValueOrDefault(EvaluationStatus.Cumplido, 0)
                                       + necesarios.GetValueOrDefault(EvaluationStatus.ParcialmenteCumplido, 0)) / totalNecesarios) * 100;
            decimal incumplidosNecesarios = (necesarios.GetValueOrDefault(EvaluationStatus.Incumplido, 0) / totalNecesarios) * 100;

            return escencialesSum >= 90 && incumplidosEscenciales <= 10 &&
                   necesariosSum >= 80 && incumplidosNecesarios <= 20;
        }

        private Dictionary<EvaluationStatus, int> ClasificarIndicadores(IEnumerable<IndicadorModel> indicadores)
        {
            // Convertimos los indicadores: si un indicador está en NoEvaluado, lo tratamos como Incumplido.
            return indicadores
                .Select(i => i.Evaluacion == EvaluationStatus.NoEvaluado ? EvaluationStatus.Incumplido : i.Evaluacion)
                .GroupBy(e => e)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }

}

/* 
 Mejoras futuras: Puedes encapsular los umbrales en constantes o incluso permitir configurarlos de forma externa (por ejemplo, en un archivo de configuración o en una base de datos) para mayor flexibilidad.
 */