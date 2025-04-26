using Tesis.Domain.Models;

namespace Tesis.Application.Services.ProcesoServices;

public static class ProcesoEvaluator
{
    public static bool EvaluarProceso(ProcesoModel Proceso , out string errorMessage)
    {
        if(Proceso is null)
        {
            errorMessage = "El Proceso no puede ser nulo";
            return false;
        }

        errorMessage = "xD";
        return true ;
    }
}
