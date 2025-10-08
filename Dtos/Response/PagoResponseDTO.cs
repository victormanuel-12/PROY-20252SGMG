using System;

namespace SGMG.Dtos.Response
{
    public class PagoResponseDTO
    {
        public int Id { get; set; }
        public string FechaPago { get; set; } = string.Empty;
        public string FechaCita { get; set; } = string.Empty;
        public string HoraCita { get; set; } = string.Empty;
        public string TipoDoc { get; set; } = string.Empty;
        public string NumeroDoc { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
