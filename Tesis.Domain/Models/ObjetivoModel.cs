using System.ComponentModel.DataAnnotations;
using Tesis.Domain.SD;

namespace Tesis.Domain.Models
{
    public class ObjetivoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        public EvaluationStatus Evaluacion { get; set; } = EvaluationStatus.NoEvaluado;

        // Colección de relación muchos‑a‑muchos con procesos:
        public ICollection<ObjetivoProcesoModel> ObjetivoProcesos { get; set; } = new List<ObjetivoProcesoModel>();

        // La relación con otros objetos (por ejemplo, con indicadores vía ObjetivoIndicadorModel)
        public ICollection<ObjetivoIndicadorModel> ObjetivoIndicadores { get; set; } = new List<ObjetivoIndicadorModel>();
    }
}
