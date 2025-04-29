using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Tesis.Domain.SD;

namespace Tesis.Domain.Models
{
    public class ProcesoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public EvaluationStatus Evaluacion { get; set; } = EvaluationStatus.NoEvaluado;

        // Cada proceso tiene una colección de indicadores
        [DisplayName("Lista de Indicadores")]
        [JsonIgnore]
        public ICollection<IndicadorModel> Indicadores { get; set; } = new List<IndicadorModel>();

        // Relación muchos‑a‑muchos con Objetivo a través de la entidad intermedia:
        public ICollection<ObjetivoProcesoModel> ObjetivoProcesos { get; set; } = new List<ObjetivoProcesoModel>();

        // Relación con los objetivos a través de la entidad intermedia ObjetivoProcesoIndicadorModel
        public ICollection<ObjetivoProcesoIndicadorModel> ObjetivoProcesoIndicadores { get; set; } = new List<ObjetivoProcesoIndicadorModel>();
    }
}
