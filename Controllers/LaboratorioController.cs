using Microsoft.AspNetCore.Mvc;
using SGMG.Dtos.Request;
using SGMG.Services;

namespace SGMG.Controllers
{
    [Route("[controller]")]
    public class LaboratorioController : Controller
    {
        private readonly ILaboratorioService _laboratorioService;

        public LaboratorioController(ILaboratorioService laboratorioService)
        {
            _laboratorioService = laboratorioService;
        }

        // Vista de listado
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Vista de nueva orden
        [HttpGet]
        [Route("nueva-orden")]
        public IActionResult NuevaOrden()
        {
            return View();
        }

        // API: Obtener historial
        [HttpGet]
        [Route("api/historial/{idPaciente}")]
        public async Task<IActionResult> GetHistorial(int idPaciente)
        {
            var response = await _laboratorioService.GetHistorialLaboratorioAsync(idPaciente);

            if (response.Success.GetValueOrDefault())
            {
                return Ok(new
                {
                    success = true,
                    paciente = response.Data?.Paciente,
                    ordenes = response.Data?.Ordenes,
                    message = response.Message
                });
            }
            else
            {
                return Ok(new { success = false, message = response.Message });
            }
        }

        // API: Crear orden
        [HttpPost]
        [Route("api/crear")]
        public async Task<IActionResult> CrearOrden([FromBody] OrdenLaboratorioRequestDTO dto)
        {
            var response = await _laboratorioService.CrearOrdenAsync(dto);
            return Ok(new
            {
                success = response.Success,
                data = response.Data,
                message = response.Message
            });
        }

        // API: Actualizar orden
        [HttpPut]
        [Route("api/actualizar")]
        public async Task<IActionResult> ActualizarOrden([FromBody] OrdenLaboratorioRequestDTO dto)
        {
            var response = await _laboratorioService.ActualizarOrdenAsync(dto);
            return Ok(new
            {
                success = response.Success,
                data = response.Data,
                message = response.Message
            });
        }

        // API: Obtener orden por ID
        [HttpGet]
        [Route("api/orden/{idOrden}")]
        public async Task<IActionResult> GetOrden(int idOrden)
        {
            var response = await _laboratorioService.GetOrdenByIdAsync(idOrden);
            return Ok(new
            {
                success = response.Success,
                data = response.Data,
                message = response.Message
            });
        }
    }
}