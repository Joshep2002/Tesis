using Mapster;
using Tesis.Application.DTOs.Indicador;
using Tesis.Application.DTOs.Objetivo;
using Tesis.Application.DTOs.Proceso;
using Tesis.Domain.Models;

namespace Tesis.Application.Services.Mapster
{
    public class MapsterConfiguration
    {
        public static void RegisterMappings()
        {
            // Configura el mapeo de ObjetivoModel a ObjetivoDTO
            TypeAdapterConfig<ObjetivoModel, ObjetivoDTO>
                    .NewConfig()    
                    .Map(dest => dest.Procesos,
                        src => src.ObjetivoProcesosIndicadores
                    .GroupBy(x => x.ProcesoId)
                    .Select(g => new MiniObjetivoDto
                    {
                        Id = g.First().Proceso.Id,
                        Nombre = g.First().Proceso.Nombre,
                        Evaluacion = g.First().Proceso.Evaluacion,
                        Indicadores = g.Where(x => x.Indicador != null)
                                       .Select(x => x.Indicador.Adapt<IndicadorDTO>())
                                       .DistinctBy(i => i.Id)
                                       .ToList()
                    })
                    .ToList());

            // Proceso
            // Configuraciones adicionales para otros mapeos, por ejemplo:
            // De IndicadorModel a IndicadorDTO, si no se resuelve automáticamente por coincidencia de nombres.
            TypeAdapterConfig<IndicadorModel, IndicadorDTO>
                .NewConfig();

            // Mapea ProcesoModel a ProcesoDTO
            TypeAdapterConfig<ProcesoModel, ProcesoDTO>
                .NewConfig()
                .Map(dest => dest.Indicadores,
                     src => src.Indicadores.Adapt<List<IndicadorDTO>>());
            // Si las propiedades coinciden en nombre, Mapster las asignará automáticamente.

            // También puedes configurar el mapeo para IndicadorModel a IndicadorDTO
            TypeAdapterConfig<IndicadorModel, IndicadorDTO>.NewConfig();

            // Indicador 
            // Mapeo de ProcesoModel a MiniProcesoDTO
            TypeAdapterConfig<ProcesoModel, MiniProcesoDTO>
                .NewConfig();

            // Mapeo de IndicadorModel a IndicadorDTO, incluyendo la propiedad anidada 'Proceso'
            TypeAdapterConfig<IndicadorModel, IndicadorDTO>
                .NewConfig()
                // La propiedad 'Proceso' se mapeará; si es null, se deja null, de lo contrario se adapta a MiniProcesoDTO.
                .Map(dest => dest.Proceso,
                     src => src.Proceso == null ? null : src.Proceso.Adapt<MiniProcesoDTO>());

            TypeAdapterConfig<IndicadorUpsertDto, IndicadorModel>
          .NewConfig();
        }
    }
}
