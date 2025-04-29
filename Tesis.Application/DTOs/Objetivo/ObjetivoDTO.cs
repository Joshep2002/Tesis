using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.Application.DTOs.Indicador;
using Tesis.Application.DTOs.Proceso;
using Tesis.Domain.Models;
using Tesis.Domain.SD;

namespace Tesis.Application.DTOs.Objetivo
{
    public class ObjetivoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public EvaluationStatus Evaluacion { get; set; }
        public List<MiniObjetivoDto> Procesos { get; set; } = new();
    }

    public class UpsertObjetivoDTO
    {
        [Required]
        public string Nombre { get; set; }
    }

    public class MiniObjetivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public EvaluationStatus Evaluacion { get; set; }
        public List<IndicadorDTO> Indicadores { get; set; } = new();
    }


}
