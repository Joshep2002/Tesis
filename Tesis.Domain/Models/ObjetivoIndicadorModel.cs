using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace Tesis.Domain.Models
{
    // Entidad intermedia que vincula Objetivo e Indicador
    // Esto permite que un mismo indicador pueda estar en varios objetivos
    // y que dentro de un objetivo se incluya (o no) cada indicador de un proceso.
    public class ObjetivoIndicadorModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ValidateNever]
        [NotNull]
        public int ObjetivoId { get; set; }

        [ValidateNever]
        [ForeignKey("ObjetivoId")]
        public ObjetivoModel Objetivo { get; set; }

        [ValidateNever]
        [NotNull]
        public int IndicadorId { get; set; }

        [ValidateNever]
        [ForeignKey("IndicadorId")]
        public IndicadorModel Indicador { get; set; }

    }
}
