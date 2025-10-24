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

        public PagoResponseDTO? GetPagoByIdDto(int id)
        {
            var p = _pagoRepository.GetPagoById(id);
            if (p == null) return null;

            var dto = new PagoResponseDTO
            {
                Id = p.IdPago,
                FechaPago = p.FechaPago.ToString("dd/MM/yyyy"),
                FechaCita = p.Cita?.FechaCita.ToString("dd/MM/yyyy") ?? string.Empty,
                HoraCita = p.Cita?.HoraCita.ToString() ?? string.Empty,
                TipoDoc = p.Cita?.Paciente?.TipoDocumento ?? string.Empty,
                NumeroDoc = p.Cita?.Paciente?.NumeroDocumento ?? string.Empty,
                Nombres = p.Cita?.Paciente != null ? string.Join(" ", new[] { p.Cita.Paciente.ApellidoPaterno, p.Cita.Paciente.ApellidoMaterno, p.Cita.Paciente.Nombre }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                Edad = p.Cita?.Paciente?.Edad ?? 0,
                Telefono = "",
                HistoriaClinicaCodigo = p.Cita?.Paciente?.HistoriasClinicas?.FirstOrDefault()?.CodigoHistoria ?? string.Empty,
                TipoSeguro = p.Cita?.Paciente?.HistoriasClinicas?.FirstOrDefault()?.TipoSeguro ?? string.Empty,
                Medico = p.Cita?.Medico != null ? string.Join(" ", new[] { p.Cita.Medico.ApellidoPaterno, p.Cita.Medico.ApellidoMaterno, p.Cita.Medico.Nombre }.Where(s => !string.IsNullOrWhiteSpace(s))) : string.Empty,
                Especialidad = p.Cita?.Especialidad ?? string.Empty,
                Consultorio = p.Cita?.Consultorio ?? string.Empty,
                Motivo = string.Empty,
                Subtotal = p.Subtotal,
                Igv = p.Igv,
                Total = p.Total,
                Estado = p.EstadoPago ?? string.Empty
            };

            // Mapear detalle del servicio desde la entidad Pago (modelo actual)
            dto.Servicios.Add(new PagoServicioDTO
            {
                Codigo = p.CodigoServicio ?? string.Empty,
                Descripcion = p.DescripcionServicio ?? string.Empty,
                Cantidad = p.Cantidad,
                PrecioUnitario = p.PrecioUnitario,
                Total = p.Total
            });

            return dto;
        }

        public bool Pagar(int id)
        {
            // marcar como pagado y actualizar fecha de pago a ahora
            var ahora = DateTime.Now;
            return _pagoRepository.UpdatePagoStatus(id, "Pagado", ahora);
        }
    }
}