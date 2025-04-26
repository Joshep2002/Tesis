using Serilog;
using System.Globalization;
using Tesis.Domain.Models;


namespace Tesis.Application.Services.IndicadorServices;

public static class IndicadorExtensions
{
    public static bool TryConvertMetaCumplir(this IndicadorModel indicador, out string errorMessage)
    {
        errorMessage = string.Empty;

        Log.Debug("Iniciando conversión de MetaCumplir para indicador {IndicadorId}. Valor: {MetaCumplir}",
        indicador?.Id,
        indicador?.MetaCumplir);

        if (!string.IsNullOrEmpty(indicador.MetaCumplir))
        {
            string metaStr = indicador.MetaCumplir.Trim();

            if (metaStr.EndsWith("%"))
            {
                indicador.IsMetaCumplirPorcentage = true;
                metaStr = metaStr.TrimEnd('%');
                Log.Debug("MetaCumplir es porcentaje. Valor limpio: {MetaStr}", metaStr);
            }
            else
            {
                indicador.IsMetaCumplirPorcentage = false;
                Log.Debug("MetaCumplir es valor absoluto. Valor: {MetaStr}", metaStr);
            }

            if (decimal.TryParse(metaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                indicador.MetaCumplirValue = result;
                Log.Information(
               "Conversión exitosa de MetaCumplir. " +
               "Indicador {IndicadorId}: {MetaStr} → {MetaValue} (Es porcentaje: {IsPorcentaje})",
               indicador.Id,
               metaStr,
               result,
               indicador.IsMetaCumplirPorcentage
           );
                return true;
            }
            else
            {
                errorMessage = "El valor de 'MetaCumplir' es inválido.";
                Log.Error(
                "Error al convertir MetaCumplir. " +
                "Indicador {IndicadorId}: Valor '{MetaStr}' no es un número válido.",
                indicador.Id,
                metaStr
            );
                return false;
            }
        }

        Log.Debug("MetaCumplir está vacío o nulo para el indicador {IndicadorId}", indicador?.Id);
        return true;
    }

    public static void NormalizarMetaReal(this IndicadorModel indicador)
    {
        if (indicador == null)
        {
            Log.Error("Error: Indicador nulo en NormalizarMetaReal");
            throw new ArgumentNullException(nameof(indicador));
        }

        Log.Debug(
       "Normalizando MetaReal para indicador {IndicadorId}. " +
       "Valor inicial: {MetaReal}, Es porcentaje: {IsPorcentaje}",
       indicador.Id,
       indicador.MetaReal,
       indicador.IsMetaCumplirPorcentage
   );

        // Quita espacios innecesarios
        string valor = indicador.MetaReal?.Trim() ?? string.Empty;

        if (indicador.IsMetaCumplirPorcentage)
        {
            // Si no termina en "%" se agrega.
            if (!valor.EndsWith("%"))
            {
                indicador.MetaReal = valor + "%";
                Log.Debug(
                "Agregado '%' a MetaReal. Indicador {IndicadorId}. Nuevo valor: {MetaReal}",
                indicador.Id,
                indicador.MetaReal
            ); 
            }
        }
        else
        {
            // Remueve cualquier símbolo "%" que pueda existir.
            indicador.MetaReal = valor.Replace("%", "");
        }
    }
}


