using Microsoft.AspNetCore.Mvc;
using SGMG.Models;
using SGMG.Services;
using SGMG.Dtos.Response;
using SGMG.Dtos.Request.Paciente;
using PROY_20252SGMG.Dtos.Response;


namespace SGMG.Controllers
{
  [Route("[controller]")]
  public class PacienteController : Controller
  {
    private readonly ILogger<PacienteController> _logger;
    private readonly IPacienteService _pacienteService;

    public PacienteController(ILogger<PacienteController> logger, IPacienteService pacienteService)
    {
      _logger = logger;
      _pacienteService = pacienteService;
    }

    [HttpGet]
    [Route("/pacientes/all")]
    public async Task<GenericResponse<IEnumerable<Paciente>>> GetAllPacientes()
    {
      return await _pacienteService.GetAllPacientesAsync();
    }

    // NUEVO: Endpoint de búsqueda
    [HttpGet]
    [Route("/pacientes/search")]
    public async Task<GenericResponse<Paciente>> SearchPaciente([FromQuery] string tipoDocumento, [FromQuery] string numeroDocumento)
    {
      return await _pacienteService.SearchPacienteByDocumentoAsync(tipoDocumento, numeroDocumento);
    }

    // NUEVO: Obtener citas pendientes de un paciente
    [HttpGet]
    [Route("/pacientes/{id}/citas-pendientes")]
    public async Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasPendientesByPaciente(int id)
    {
      return await _pacienteService.GetCitasPendientesByPacienteAsync(id);
    }

    [HttpGet]
    [Route("/pacientes/{id}")]
    public async Task<GenericResponse<Paciente>> GetPacienteById(int id)
    {
      return await _pacienteService.GetPacienteByIdAsync(id);
    }

    [HttpPost]
    [Route("/pacientes/register")]
    public async Task<GenericResponse<Paciente>> CreatePaciente([FromBody] PacienteRequestDTO pacienteRequestDTO)
    {
      return await _pacienteService.AddPacienteAsync(pacienteRequestDTO);
    }

    [HttpPut]
    [Route("/pacientes/update")]
    public async Task<GenericResponse<Paciente>> UpdatePaciente([FromBody] PacienteRequestDTO pacienteRequestDTO)
    {
      return await _pacienteService.UpdatePacienteAsync(pacienteRequestDTO);
    }

    [HttpDelete]
    [Route("/pacientes/delete/{id}")]
    public async Task<GenericResponse<Paciente>> DeletePaciente(int id)
    {
      return await _pacienteService.DeletePacienteAsync(id);
    }

    [HttpGet]
    [Route("/pacientes/{id}/derivaciones")]
    public async Task<GenericResponse<IEnumerable<DerivacionResponseDTO>>> GetDerivacionesByPaciente(int id)
    {
      return await _pacienteService.GetDerivacionesByPacienteAsync(id);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View("Error!");
    }
  }
}
