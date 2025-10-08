// ...existing code...
using Microsoft.AspNetCore.Mvc;
using SGMG.Services;
using SGMG.common.exception;
using System;
using System.Linq;
using System.Collections.Generic;
using SGMG.Dtos.Response; // <-- agregado
// ...existing code...
namespace SGMG.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoService _pagoService;
        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

    [HttpGet]
    [NoValidation]
    [Route("/pagos/all")]
    public IActionResult Historial(string tipoBusqueda, string numeroDocumento, DateTime? inicio, DateTime? fin, string estado)
        {
            // El controlador delega la lógica al servicio
            var tipoDocumento = tipoBusqueda;
            var pagosDto = _pagoService.GetHistorialPagosDto(tipoDocumento, numeroDocumento, inicio, fin, estado);
            // La vista real está en Views/Home/Historial.cshtml, indicar la ruta completa para evitar búsquedas en /Views/Pago/
            var model = pagosDto ?? new List<PagoResponseDTO>();
            return View("~/Views/Home/Historial.cshtml", model);
        }

    [HttpGet]
    [Route("/pagos/search")]
    public ActionResult<GenericResponse<IEnumerable<PagoResponseDTO>>> Search(string tipoBusqueda, string numeroDocumento, DateTime? inicio, DateTime? fin, string estado)
        {
            var tipoDocumento = tipoBusqueda;
            var pagosDto = _pagoService.GetHistorialPagosDto(tipoDocumento, numeroDocumento, inicio, fin, estado) ?? Enumerable.Empty<PagoResponseDTO>();
            var resp = new GenericResponse<IEnumerable<PagoResponseDTO>>
            {
                Success = true,
                Message = "OK",
                Data = pagosDto
            };
            return Ok(resp);
        }
    }
}
