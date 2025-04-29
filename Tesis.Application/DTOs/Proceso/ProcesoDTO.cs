using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tesis.Domain.SD;
using Tesis.Application.DTOs.Indicador;

namespace Tesis.Application.DTOs.Proceso
{
    public class ProcesoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public EvaluationStatus Evaluacion { get; set; } = EvaluationStatus.NoEvaluado;
        public ICollection<IndicadorDTO>? Indicadores { get; set; } = new List<IndicadorDTO>();
    }
    public class MiniProcesoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public EvaluationStatus Evaluacion { get; set; } = EvaluationStatus.NoEvaluado;
    }

    public class ProcesoUpsertDTO
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Nombre muy largo")]
        [DisplayName("Nombre")]
        public string Nombre { get; set; } = string.Empty;

    }
}
