using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tesis.Domain.SD;

namespace Tesis.Domain.Models
{
    public class ObjetivoModel
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public EvaluationStatus Evaluacion { get; set; } = EvaluationStatus.NoEvaluado;

        // Colección de relación muchos‑a‑muchos con procesos:
        public ICollection<ObjetivoProcesoModel> ObjetivoProcesos { get; set; } = new List<ObjetivoProcesoModel>();

        // La relación con otros objetos (por ejemplo, con indicadores vía ObjetivoIndicadorModel)
        public ICollection<ObjetivoIndicadorModel> ObjetivoIndicadores { get; set; } = new List<ObjetivoIndicadorModel>();

        // Relación con los objetivos a través de la entidad intermedia
        public ICollection<ObjetivoProcesoIndicadorModel> ObjetivoProcesosIndicadores { get; set; } = new List<ObjetivoProcesoIndicadorModel>();
    }
}
