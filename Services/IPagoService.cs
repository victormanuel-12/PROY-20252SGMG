using SGMG.Models;
using System;
using System.Collections.Generic;

namespace SGMG.Services
{
    public interface IPagoService
    {
        IEnumerable<Pago> GetHistorialPagos(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado);

        // Nuevo: devolver DTOs listos para el controller o para serializar
        IEnumerable<SGMG.Dtos.Response.PagoResponseDTO> GetHistorialPagosDto(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado);
    }
}