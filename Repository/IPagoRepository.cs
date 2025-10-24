using SGMG.Models;
using System;
using System.Collections.Generic;

namespace SGMG.Repository
{
    public interface IPagoRepository
    {
        IEnumerable<Pago> GetPagosByFiltro(string tipoDocumento, string numeroDocumento, DateTime? fechaInicio, DateTime? fechaFin, string estado);
        Pago? GetPagoById(int id);
        bool UpdatePagoStatus(int id, string estado, DateTime? fechaPago = null);
    }
}