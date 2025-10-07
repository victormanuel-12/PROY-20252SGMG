using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGMG.Models;
using SGMG.Services;
using SGMG.Dtos.Response;
using SGMG.Dtos.Request.Medico;

namespace SGMG.Controllers
{
  [Route("api/[controller]")]
  public class MedicoController : Controller
  {
    private readonly ILogger<MedicoController> _logger;
    private readonly IMedicoService _medicoService;

    public MedicoController(ILogger<MedicoController> logger, IMedicoService medicoService)
    {
      _logger = logger;
      _medicoService = medicoService;
    }

    [HttpGet]
    [Route("/medicos/all")]
    public async Task<GenericResponse<IEnumerable<Medico>>> GetAllMedicos()
    {
      return await _medicoService.GetAllMedicosAsync();
    }

    [HttpGet]
    [Route("/medicos/{id}")]
    public async Task<GenericResponse<Medico>> GetMedicoById(int id)
    {
      return await _medicoService.GetMedicoByIdAsync(id);
    }

    [HttpPost]
    [Route("/medicos/register")]
    public async Task<GenericResponse<Medico>> CreateMedico([FromBody] MedicoRequestDTO medicoRequestDTO)
    {
      return await _medicoService.AddMedicoAsync(medicoRequestDTO);
    }

    [HttpPut]
    [Route("/medicos/update")]
    public async Task<GenericResponse<Medico>> UpdateMedico([FromBody] MedicoRequestDTO medicoRequestDTO)
    {
      return await _medicoService.UpdateMedicoAsync(medicoRequestDTO);
    }

    [HttpDelete]
    [Route("/medicos/delete/{id}")]
    public async Task<GenericResponse<Medico>> DeleteMedico(int id)
    {
      return await _medicoService.DeleteMedicoAsync(id);
    }

    [HttpGet]
    [Route("/medicos/filters")]
    public async Task<GenericResponse<IEnumerable<Medico>>> GetMedicosFiltered([FromQuery] string? numeroDni, [FromQuery] string? idConsultorio, [FromQuery] string? estado, [FromQuery] string? fechaInicio, [FromQuery] string? fechaFin, [FromQuery] string? turno)
    {
      return await _medicoService.GetMedicosFilteredAsync(numeroDni, idConsultorio, estado, fechaInicio, fechaFin, turno);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View("Error!");
    }
  }
}