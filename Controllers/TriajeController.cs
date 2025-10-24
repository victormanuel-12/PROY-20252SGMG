using Microsoft.AspNetCore.Mvc;
using SGMG.Models;
using SGMG.Services;
using SGMG.Dtos.Response;
using SGMG.Dtos.Request.Triaje;

namespace SGMG.Controllers
{
    [Route("[controller]")]
    public class TriajeController : Controller
    {
        private readonly ILogger<TriajeController> _logger;
        private readonly ITriajeService _triajeService;

        public TriajeController(ILogger<TriajeController> logger, ITriajeService triajeService)
        {
            _logger = logger;
            _triajeService = triajeService;
        }

        // ============== RUTAS PARA VISTAS ==============
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/triaje/listado")]
        public IActionResult Listado()
        {
            return View();
        }

        [HttpGet]
        [Route("/triaje/registrar")]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpGet]
        [Route("/triaje/editar")]
        public IActionResult Editar()
        {
            return View();
        }

        // ============== ENDPOINTS API ==============


        [HttpGet]
        [Route("/triaje/buscar")]
        public async Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> BuscarTriajes([FromQuery] string? tipoDoc,[FromQuery] string? numeroDoc,[FromQuery] DateTime? fechaInicio,[FromQuery] DateTime? fechaFin)
        {
            return await _triajeService.BuscarTriajesAsync(tipoDoc, numeroDoc, fechaInicio, fechaFin);
        }

        [HttpPost]
        [Route("/triaje/register")]
        public async Task<GenericResponse<Triaje>> CreateTriaje([FromBody] TriajeRequestDTO triajeRequestDTO)
        {
            return await _triajeService.AddTriajeAsync(triajeRequestDTO);
        }

        [HttpGet]
        [Route("/triaje/{id}")]
        public async Task<GenericResponse<TriajeResponseDTO>> GetTriajeById(int id)
        {
            return await _triajeService.GetTriajeByIdAsync(id);
        }

        [HttpPut]
        [Route("/triaje/update")]
        public async Task<GenericResponse<TriajeResponseDTO>> UpdateTriaje([FromBody] TriajeRequestDTO triajeRequestDTO)
        {
            return await _triajeService.UpdateTriajeAsync(triajeRequestDTO);
        }

        [HttpGet]
        [Route("/triaje/all")]
        public async Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> GetAllTriajes()
        {
            return await _triajeService.GetAllTriajesAsync();
        }
    }
}