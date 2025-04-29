using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Tesis.Domain.SD;

namespace Tesis.Domain.Models
{
    public class IndicadorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Descripcion { get; set; }

        private string _metaCumplir = string.Empty;
        public string MetaCumplir
        {
            get => _metaCumplir;
            set
            {
                _metaCumplir = value;

                ConvertMetaCumplir();

                if (!string.IsNullOrEmpty(MetaReal))
                {
                    MetaReal = MetaReal; // Dispara normalización y conversión
                }
            }
        }
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
                // Normalizar primero
                _metaReal = NormalizarValorMetaReal(value);

                // Convertir a decimal después
                ConvertMetaReal();

                // Actualizar evaluación si MetaReal está vacía
                if (string.IsNullOrEmpty(value))
                {
                    _evaluacion = EvaluationStatus.NoEvaluado;
                }
                Evaluar();
            }
        }
        public decimal MetaRealValue { get; set; } = 0;
        public bool IsMetaRealPorcentage { get; set; } = false;

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

        // Relación con los objetivos a través de la entidad intermedia ObjetivoProcesoIndicadorModel
        public ICollection<ObjetivoProcesoIndicadorModel> ObjetivoProcesoIndicadores { get; set; } = new List<ObjetivoProcesoIndicadorModel>();

        private void ConvertMetaCumplir()
        {
            if (string.IsNullOrEmpty(MetaCumplir)) return;

            var metaStr = MetaCumplir.Trim();
            IsMetaCumplirPorcentage = metaStr.EndsWith("%");

            if (IsMetaCumplirPorcentage)
                metaStr = metaStr.TrimEnd('%');

            if (decimal.TryParse(metaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                MetaCumplirValue = result;
            }
            else
            {
                throw new ArgumentException("Formato inválido para MetaCumplir");
            }
        }

        public void Evaluar()
        {
            if (string.IsNullOrEmpty(MetaReal))
            {
                Evaluacion = EvaluationStatus.NoEvaluado;
                return;
            }

            ConvertMetaReal();

            if (MetaCumplirValue <= 0)
            {
                throw new InvalidOperationException("MetaCumplirValue debe ser mayor a cero");
            }

            decimal porcentaje = (MetaRealValue / MetaCumplirValue) * 100;

            Evaluacion = porcentaje switch
            {
                > 100 => EvaluationStatus.SobreCumplido,
                100 => EvaluationStatus.Cumplido,
                >= 80 => EvaluationStatus.ParcialmenteCumplido,
                _ => EvaluationStatus.Incumplido
            };
        }

        private void ConvertMetaReal()
        {
            if (string.IsNullOrEmpty(_metaReal)) return;

            var metaStr = _metaReal.Trim();

            IsMetaRealPorcentage = metaStr.EndsWith("%");

            if (IsMetaRealPorcentage)
            {
                metaStr = metaStr.TrimEnd('%');
            }
            
            if (!decimal.TryParse(metaStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            {
                throw new FormatException($"MetaReal inválido: {_metaReal}");
            }

            MetaRealValue = value;
        }

        private string NormalizarValorMetaReal(string valor)
        {
            if (IsMetaCumplirPorcentage)  // Depende de MetaCumplir
            {
                if (string.IsNullOrEmpty(_metaReal)) return _metaReal;

                IsMetaRealPorcentage = true;

                return valor?.Trim().EndsWith("%") == true ? valor.Trim() : $"{valor?.Trim()}%";
            }
            else
            {
                return valor?.Trim().Replace("%", "") ?? "";
            }
        }

    }
}
