using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Domain.SD;

namespace Tesis.Application.DTOs.Proceso
{
    public class ProcesoDTO
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
