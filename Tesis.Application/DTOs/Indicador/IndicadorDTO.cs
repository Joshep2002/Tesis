using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tesis.Application.DTOs.Proceso;
using Tesis.Domain.SD;

namespace Tesis.Application.DTOs.Indicador
{
    public class IndicadorDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string MetaCumplir { get; set; } = string.Empty;
        public decimal MetaCumplirValue { get; set; }
        public string MetaReal { get; set; } = string.Empty;
        public decimal MetaRealValue { get; set; }
        public IndicadorType Tipo { get; set; }
        public EvaluationStatus Evaluacion { get; set; }
        public MiniProcesoDTO? Proceso { get; set; }
    }
    public class MiniIndicadorDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public EvaluationStatus Evaluacion { get; set; }
        
    }

    public class IndicadorUpsertDto
    {
        [Required]
        [DisplayName("Descripcion")]
        [StringLength(200, ErrorMessage = "Descripcion muy larga")]
        public string Descripcion { get; set; } = string.Empty;

        [MaxLength(6, ErrorMessage = "Numero muy grande. Ejemplo: 50.04%")]
        [RegularExpression(@"^\d+(\.\d+)?%?$", ErrorMessage = "Debe ser un numero decimal .Opcional(Terminar en %)")]
        [Required(ErrorMessage = "Este campo no puede ser nulo ni vacio")]
        [DisplayName("Meta a cumplir")]
        public string MetaCumplir { get; set; } = string.Empty;

        [MaxLength(6, ErrorMessage = "Numero muy grande. Ejemplo: 50.04%")]
        [RegularExpression(@"^\d+(\.\d+)?%?$", ErrorMessage = "Debe ser un numero decimal .Opcional(Terminar en %)")]
        [DisplayName("Meta real")]
        public string MetaReal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar Escencial/Necesario")]
        [DisplayName("Tipo de Indicador")]
        public IndicadorType Tipo { get; set; }
    }
}


