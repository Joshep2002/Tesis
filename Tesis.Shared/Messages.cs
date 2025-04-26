using Tesis.Domain.Models;

namespace Tesis.Shared
{
    public static class Messages
    {
        public static void AllIndicadoresConsoleMessage(this IEnumerable<IndicadorModel> Indicadores)
        {
            Console.WriteLine("----------------LISTA DE INDICADORES----------------------");

            foreach (var indicador in Indicadores)
            {
                Console.WriteLine($"" +
                   $"Indicador#{indicador.Id}\n" +
                   $"Descripcion: {indicador.Descripcion}\n" +
                   $"Meta a Cumplir: {indicador.MetaCumplir}\n" +
                   $"Meta Real: {indicador.MetaReal}\n" +
                   $"Evaluacion: {indicador.Evaluacion}");
                Console.Write("Proceso: ");
                if (indicador.Proceso is null) Console.WriteLine("Vacio");
                else Console.WriteLine($"{indicador.Proceso.Nombre}");
                Console.WriteLine("---------------------------------------------------");
            }


        }

        public static void AllProcesosConsoleMessage(this IEnumerable<ProcesoModel> Procesos)
        {
            Console.WriteLine("----------------LISTA DE PROCESOS----------------------");
            List<int> cantIndicadores = new() ;

            foreach (var proceso in Procesos)
            {
                Console.WriteLine($"" +
               $"Proceso#{proceso.Id}\n" +
               $"Nombre: {proceso.Nombre}\n" +
               $"Evaluacion: {proceso.Evaluacion}"
               );

                foreach (var indicadorProceso in proceso.Indicadores)
                {
                    cantIndicadores.Add(indicadorProceso.Id);
                }

                Console.Write("Indicadores: |");
                if (cantIndicadores.Count == 0) Console.Write("Vacio|");

                foreach (var cant in cantIndicadores)
                {
                    
                     Console.Write($"{cant}|");
                }



                Console.WriteLine();
                cantIndicadores.Clear();
                Console.WriteLine("---------------------------------------------------");
            }


        }

        public static void ProcesoCreadoConsoleMessage(this ProcesoModel Proceso)
        {
            Console.WriteLine("Proceso Creado");
        }

    }
}
