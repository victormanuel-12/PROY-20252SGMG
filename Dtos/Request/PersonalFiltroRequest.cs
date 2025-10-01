using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SGMG.Dtos.Request
{
  public class PersonalFiltroRequest
  {
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string? Nombre { get; set; }
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string? Dni { get; set; }
    public string? Estado { get; set; }
    public string? TipoPersonal { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "El IdConsultorio debe ser un número entero positivo.")]
    public int? IdConsultorio { get; set; }
  }
}