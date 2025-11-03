using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGMG.Dtos.Request.Triaje
{
    public class TriajeRequestDTO
    {
        public int IdTriaje { get; set; }
        public int IdPaciente { get; set; }
        
    [Required(ErrorMessage = "La temperatura es obligatoria")]
    [Range(35, 42, ErrorMessage = "La temperatura debe estar entre 35°C y 42°C")]
    public decimal Temperatura { get; set; }
    
    [Required(ErrorMessage = "La presión arterial es obligatoria")]
    [Range(60, 200, ErrorMessage = "La presión arterial debe estar entre 60 y 200")]
    public int PresionArterial { get; set; }
    
    [Required(ErrorMessage = "La saturación es obligatoria")]
    [Range(70, 100, ErrorMessage = "La saturación debe estar entre 70% y 100%")]
    public int Saturacion { get; set; }
    
    // Agregar similar para los otros campos requeridos
    [Required(ErrorMessage = "La frecuencia cardíaca es obligatoria")]
    public int FrecuenciaCardiaca { get; set; }
    
    [Required(ErrorMessage = "La frecuencia respiratoria es obligatoria")]
    public int FrecuenciaRespiratoria { get; set; }
        public decimal Peso { get; set; }
        public decimal Talla { get; set; }
        public decimal PerimAbdominal { get; set; }
        public decimal SuperficieCorporal { get; set; }
        public decimal Imc { get; set; }
        public string ClasificacionImc { get; set; } = "";
        public string RiesgoEnfermedad { get; set; } = "";
        public string EstadoTriage { get; set; } = "";
        public string Observaciones { get; set; } = "";
    }
}