using Serilog;
using System.Globalization;
using Tesis.Domain.Models;
using Tesis.Domain.SD;



namespace Tesis.Application.Services.IndicadorServices;

public static class IndicadorEvaluator
{
    /// <summary>
    /// Evalúa la propiedad MetaReal del indicador y actualiza su evaluación.
    /// Se calcula el porcentaje de cumplimiento a partir de la fórmula:
    /// (MetaRealValue / MetaCumplirValue) * 100 y se asigna el estado según:
    /// - > 100: SobreCumplido
    /// - == 100: Cumplido
    /// - >= 80 y < 100: ParcialmenteCumplido
    /// - < 80: Incumplido
    /// </summary>
    /// <param name="indicador">Indicador a evaluar</param>
    /// <param name="errorMessage">Mensaje de error en caso de fallo en la conversión</param>
    /// <returns>true si se realizó la evaluación correctamente, false en caso de error</returns>
    public static bool EvaluarIndicador(IndicadorModel indicador, out string errorMessage)
    {
        errorMessage = string.Empty;

        // Log de entrada al método (nivel Debug)
        Log.Debug("Iniciando evaluación del indicador {@IndicadorId}", indicador?.Id);

        if (indicador == null)
        {
            errorMessage = "El indicador no puede ser nulo.";
            Log.Error("Error: Indicador nulo recibido");
            return false;
        }

        if (string.IsNullOrEmpty(indicador.MetaReal))
        {
            errorMessage = "La propiedad MetaReal no puede estar vacía.";
            Log.Warning("Advertencia: MetaReal vacía en indicador {IndicadorId}", indicador.Id);
            return false;
        }

        // Convertir el valor string de MetaReal a decimal, considerando si trae '%' al final.
        string metaRealStr = indicador.MetaReal.Trim();
        if (metaRealStr.EndsWith("%"))
        {
            indicador.IsMetaRealPorcentage = true;
            metaRealStr = metaRealStr.TrimEnd('%').Trim();
            Log.Debug("MetaReal es un porcentaje: {MetaRealStr}%", metaRealStr);
        }
        else
        {
            indicador.IsMetaRealPorcentage = false;
            Log.Debug("MetaReal es un valor absoluto: {MetaRealStr}", metaRealStr);
        }

        if (!decimal.TryParse(metaRealStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal metaRealDecimal))
        {
            errorMessage = "El valor de MetaReal es inválido.";
            Log.Error("Error: MetaReal inválida ({MetaRealStr}) en indicador {IndicadorId}", metaRealStr, indicador.Id);
            return false;
        }
        indicador.MetaRealValue = metaRealDecimal;
        Log.Debug("MetaReal convertida a decimal: {MetaRealDecimal}", metaRealDecimal);

        // Validar que MetaCumplirValue esté definido y sea mayor a cero para evitar división por cero.
        if (indicador.MetaCumplirValue <= 0)
        {
            errorMessage = "MetaCumplirValue debe ser mayor a cero para realizar la evaluación.";

            Log.Error(
          "Error: MetaCumplirValue <= 0 (Valor: {MetaCumplirValue}) en indicador {IndicadorId}",
          indicador.MetaCumplirValue,
          indicador.Id);

            return false;
        }

        // Calcular el porcentaje de cumplimiento.
        decimal porcentajeCumplimiento = (metaRealDecimal / indicador.MetaCumplirValue) * 100;

        Log.Information(
        "Cálculo de cumplimiento: {MetaRealDecimal} / {MetaCumplirValue} = {Porcentaje}%",
        metaRealDecimal,
        indicador.MetaCumplirValue,
        porcentajeCumplimiento
    );

        // Asignar la Evaluacion según el porcentaje.
        if (porcentajeCumplimiento > 100)
        {
            indicador.Evaluacion = EvaluationStatus.SobreCumplido;
            Log.Information("Indicador {IndicadorId} SOBRECUMPLIDO ({Porcentaje}%)", indicador.Id, porcentajeCumplimiento);
        }
        else if (porcentajeCumplimiento == 100)
        {
            indicador.Evaluacion = EvaluationStatus.Cumplido;
            Log.Information("Indicador {IndicadorId} CUMPLIDO al 100%", indicador.Id);
        }
        else if (porcentajeCumplimiento >= 80)
        {
            indicador.Evaluacion = EvaluationStatus.ParcialmenteCumplido;
            Log.Information("Indicador {IndicadorId} PARCIALMENTE CUMPLIDO ({Porcentaje}%)", indicador.Id, porcentajeCumplimiento);
        }
        else
        {
            indicador.Evaluacion = EvaluationStatus.Incumplido;
            Log.Warning("Indicador {IndicadorId} INCUMPLIDO ({Porcentaje}%)", indicador.Id, porcentajeCumplimiento);
        }

       
        Log.Debug("Evaluación finalizada para el indicador {IndicadorId}", indicador.Id);
        return true;
    }
}

