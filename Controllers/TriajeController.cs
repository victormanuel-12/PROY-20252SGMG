using Microsoft.AspNetCore.Mvc;
using SGMG.Models;
using SGMG.Services;
using SGMG.Dtos.Response;
using SGMG.Dtos.Request.Triaje;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGMG.Data;

using PROY_20252SGMG.Models;

namespace SGMG.Controllers
{
  [Route("[controller]")]
  public class TriajeController : Controller
  {
    private readonly ILogger<TriajeController> _logger;
    private readonly ITriajeService _triajeService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TriajeController(
        ILogger<TriajeController> logger,
        ITriajeService triajeService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
      _logger = logger;
      _triajeService = triajeService;
      _userManager = userManager;
      _context = context;
    }

    // ============== RUTAS PARA VISTAS ==============

    [HttpGet]
    public async Task<IActionResult> Index()
    {
      await CargarDatosEnfermera();
      return View();
    }

    [HttpGet]
    [Route("/triaje/listado")]
    public async Task<IActionResult> Listado()
    {
      await CargarDatosEnfermera();
      return View();
    }

    [HttpGet]
    [Route("/triaje/registrar")]
    public async Task<IActionResult> Registrar()
    {
      await CargarDatosEnfermera();
      return View();
    }

    [HttpGet]
    [Route("/triaje/editar")]
    public async Task<IActionResult> Editar()
    {
      await CargarDatosEnfermera();
      return View();
    }

    // ============== MÉTODO PRIVADO PARA CARGAR DATOS ==============

    private async Task CargarDatosEnfermera()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user != null)
      {
        var enfermera = await _context.Enfermerias.Where(e => e.IdEnfermeria.ToString() == user.IdUsuario)
                            .Include(e => e.Personal)
                            .FirstOrDefaultAsync();

        if (enfermera != null)
        {
          var nombreCompleto = $"{enfermera.Personal.Nombre} {enfermera.Personal.ApellidoPaterno} {enfermera.Personal.ApellidoMaterno}";
          ViewData["NombreEnfermera"] = nombreCompleto;
          ViewData["NumeroColegiaturaEnfermeria"] = enfermera.NumeroColegiaturaEnfermeria;
          ViewData["NivelProfesional"] = enfermera.NivelProfesional;
        }
      }
    }

    // ============== ENDPOINTS API ==============

    [HttpGet]
    [Route("/triaje/buscar")]
    public async Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> BuscarTriajes(
        [FromQuery] string? tipoDoc,
        [FromQuery] string? numeroDoc,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
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