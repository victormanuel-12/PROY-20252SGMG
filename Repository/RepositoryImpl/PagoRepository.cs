using SGMG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SGMG.Data;

namespace SGMG.Repository.RepositoryImpl
{
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _context;
        public PagoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Pago> GetPagosByFiltro(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            // Incluir relaciones para evitar problemas de carga perezosa en la vista
            var query = _context.Pagos
                .Include(p => p.Cita)
                    .ThenInclude(c => c.Paciente)
                .AsQueryable();

            // Aplicar filtros sólo si vienen provistos (permitir búsqueda general si están vacíos)
            if (!string.IsNullOrWhiteSpace(tipoDocumento))
                query = query.Where(p => p.Cita != null && p.Cita.Paciente != null && p.Cita.Paciente.TipoDocumento == tipoDocumento);
            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                query = query.Where(p => p.Cita != null && p.Cita.Paciente != null && p.Cita.Paciente.NumeroDocumento == numeroDocumento);

            if (fechaInicio.HasValue)
                query = query.Where(p => p.FechaPago >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(p => p.FechaPago <= fechaFin.Value);
            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
                query = query.Where(p => p.EstadoPago == estado);

            return query
                .OrderByDescending(p => p.FechaPago)
                .ToList();
        }
    }
}


