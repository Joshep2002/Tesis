using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Tesis.Domain.Models
{
    public class ObjetivoProcesoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ObjetivoId { get; set; }
        [ForeignKey("ObjetivoId")]
        public ObjetivoModel Objetivo { get; set; }

        [Required]
        public int ProcesoId { get; set; }
        [ForeignKey("ProcesoId")]
        public ProcesoModel Proceso { get; set; }

        // Aquí podrías agregar propiedades adicionales, por ejemplo:
        // public DateTime FechaAsignacion { get; set; }
    }
}
