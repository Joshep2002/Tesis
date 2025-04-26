using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Tesis.Domain.SD;

namespace Tesis.Domain.Models
{
    public class IndicadorModel
    {
        [Key] public int Id { get; set; }
        public string Descripcion { get; set; }
        public string MetaCumplir { get; set; } 
        public decimal MetaCumplirValue { get; set; } = 0; 
        public bool IsMetaCumplirPorcentage { get; set; } = false;

        private string _metaReal = string.Empty;

        [DisplayName("Meta real")]
        public string MetaReal
        {
            get => _metaReal;
            set
            {
                _metaReal = value;

                // Actualizar evaluación si MetaReal está vacía
                if (string.IsNullOrEmpty(value))
                {
                    _evaluacion = EvaluationStatus.NoEvaluado;
                }
            }
        }
        public decimal MetaRealValue { get; set; } = 0; 
        public bool IsMetaRealPorcentage { get; set; }

        private EvaluationStatus _evaluacion = EvaluationStatus.NoEvaluado;

        [DisplayName("Evaluacion")]
        public EvaluationStatus Evaluacion
        {
            get => _evaluacion;
            set
            {
                // Solo actualizar si MetaReal no está vacía
                if (!string.IsNullOrEmpty(MetaReal))
                {
                    _evaluacion = value;
                }
            }
        }
        public IndicadorType Tipo { get; set; }

        // Relación: cada indicador pertenece a un único proceso.
        [ValidateNever]
        [NotNull]
        public int? ProcesoId { get; set; }

        [ValidateNever]
        [ForeignKey("ProcesoId")]
        public ProcesoModel Proceso { get; set; }

        // Relación muchos‑a‑muchos con Objetivo (usando join entity)
        public ICollection<ObjetivoIndicadorModel> ObjetivoIndicadores { get; set; } = new List<ObjetivoIndicadorModel>();
    }
}
