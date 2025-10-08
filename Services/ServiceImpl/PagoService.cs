using SGMG.Models;
using SGMG.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using SGMG.Dtos.Response;

namespace SGMG.Services.ServiceImpl
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        public PagoService(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        public IEnumerable<Pago> GetHistorialPagos(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            return _pagoRepository.GetPagosByFiltro(tipoDocumento, numeroDocumento, fechaInicio, fechaFin, estado);
        }

        public IEnumerable<PagoResponseDTO> GetHistorialPagosDto(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            var pagos = _pagoRepository.GetPagosByFiltro(tipoDocumento, numeroDocumento, fechaInicio, fechaFin, estado);
            return pagos.Select(p => new PagoResponseDTO {
                Id = p.IdPago,
                FechaPago = p.FechaPago.ToString("dd/MM/yyyy"),
                FechaCita = p.Cita?.FechaCita.ToString("dd/MM/yyyy") ?? string.Empty,
                HoraCita = p.Cita?.HoraCita.ToString() ?? string.Empty,
                TipoDoc = p.Cita?.Paciente?.TipoDocumento ?? string.Empty,
                NumeroDoc = p.Cita?.Paciente?.NumeroDocumento ?? string.Empty,
                Nombres = p.Cita?.Paciente != null ? string.Join(" ", new[] { p.Cita.Paciente.ApellidoPaterno, p.Cita.Paciente.ApellidoMaterno, p.Cita.Paciente.Nombre }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                Total = p.Total,
                Estado = p.EstadoPago ?? string.Empty
            }).ToList();
        }
    }
}