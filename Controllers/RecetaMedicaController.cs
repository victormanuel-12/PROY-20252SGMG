using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGMG.Dtos.Response;
using PROY_20252SGMG.Dtos.Request;
using PROY_20252SGMG.Dtos.Response;
using PROY_20252SGMG.Services;

namespace PROY_20252SGMG.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class RecetaMedicaController : ControllerBase
  {
    private readonly ILogger<RecetaMedicaController> _logger;
    private readonly IRecetaService _recetaService;

    public RecetaMedicaController(ILogger<RecetaMedicaController> logger, IRecetaService recetaService)
    {
      _logger = logger;
      _recetaService = recetaService;
    }

    /// <summary>
    /// Agregar un medicamento individual con estado PENDIENTE
    /// </summary>
    [HttpPost]
    [Route("AgregarMedicamento")]
    public async Task<ActionResult<GenericResponse<DetalleRecetaResponseDTO>>> AgregarMedicamento(
        [FromBody] AgregarMedicamentoRequestDTO request)
    {
      try
      {
        var result = await _recetaService.AgregarMedicamentoPendienteAsync(request);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al agregar medicamento");
        return BadRequest(new GenericResponse<DetalleRecetaResponseDTO>
        {
          Success = false,
          Message = "Error al agregar medicamento: " + ex.Message
        });
      }
    }

    /// <summary>
    /// Obtener medicamentos PENDIENTES por HC, Médico y Paciente
    /// </summary>
    [HttpGet]
    [Route("MedicamentosPendientes")]
    public async Task<ActionResult<GenericResponse<IEnumerable<DetalleRecetaResponseDTO>>>> GetMedicamentosPendientes(
        [FromQuery] int idCita,
        [FromQuery] int idMedico,
        [FromQuery] int idPaciente,
        [FromQuery] int? idHistoriaClinica = null)
    {
      try
      {
        var result = await _recetaService.GetMedicamentosPendientesAsync(
            idCita, idMedico, idPaciente, idHistoriaClinica);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al obtener medicamentos pendientes");
        return BadRequest(new GenericResponse<IEnumerable<DetalleRecetaResponseDTO>>
        {
          Success = false,
          Message = "Error: " + ex.Message
        });
      }
    }

    /// <summary>
    /// Eliminar un medicamento PENDIENTE
    /// </summary>
    [HttpDelete]
    [Route("EliminarMedicamento/{idDetalle}")]
    public async Task<ActionResult<GenericResponse<bool>>> EliminarMedicamento(int idDetalle)
    {
      try
      {
        var result = await _recetaService.EliminarMedicamentoPendienteAsync(idDetalle);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al eliminar medicamento");
        return BadRequest(new GenericResponse<bool>
        {
          Success = false,
          Message = "Error: " + ex.Message
        });
      }
    }

    /// <summary>
    /// Guardar receta: Cambia todos los medicamentos PENDIENTES a COMPLETADO
    /// </summary>
    [HttpPost]
    [Route("GuardarReceta")]
    public async Task<ActionResult<GenericResponse<RecetaResponseDTO>>> GuardarReceta(
        [FromBody] RecetaRequestDTO request)
    {
      try
      {
        var result = await _recetaService.CompletarRecetaAsync(request);
        return Ok(result);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error al guardar receta");
        return BadRequest(new GenericResponse<RecetaResponseDTO>
        {
          Success = false,
          Message = "Error al guardar la receta: " + ex.Message
        });
      }
    }
    [HttpGet]
    [Route("paciente/{idPaciente}/lista")]
    public async Task<ActionResult<GenericResponse<IEnumerable<RecetaResponseDTO>>>> GetRecetasListByPaciente(int idPaciente)
    {
      var result = await _recetaService.GetRecetasByPacienteAsyncCompleto(idPaciente);
      return Ok(result);
    }

    /// <summary>
    /// Obtener receta por ID
    /// </summary>
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<GenericResponse<RecetaResponseDTO>>> GetRecetaById(int id)
    {
      var result = await _recetaService.GetRecetaByIdAsync(id);
      return Ok(result);
    }

    /// <summary>
    /// Obtener recetas por paciente
    /// </summary>
    [HttpGet]
    [Route("paciente/{idPaciente}")]
    public async Task<ActionResult<GenericResponse<IEnumerable<RecetaResponseDTO>>>> GetRecetasByPaciente(int idPaciente)
    {
      var result = await _recetaService.GetRecetasByPacienteAsync(idPaciente);
      return Ok(result);
    }

    /// <summary>
    /// Obtener recetas por médico
    /// </summary>
    [HttpGet]
    [Route("medico/{idMedico}")]
    public async Task<ActionResult<GenericResponse<IEnumerable<RecetaResponseDTO>>>> GetRecetasByMedico(int idMedico)
    {
      var result = await _recetaService.GetRecetasByMedicoAsync(idMedico);
      return Ok(result);
    }

    /// <summary>
    /// Obtener receta por cita
    /// </summary>
    [HttpGet]
    [Route("cita/{idCita}")]
    public async Task<ActionResult<GenericResponse<RecetaResponseDTO>>> GetRecetaByCita(int idCita)
    {
      var result = await _recetaService.GetRecetaByCitaAsync(idCita);
      return Ok(result);
    }

    /// <summary>
    /// Marcar receta como impresa
    /// </summary>
    [HttpPut]
    [Route("imprimir/{id}")]
    public async Task<ActionResult<GenericResponse<bool>>> ImprimirReceta(int id)
    {
      var result = await _recetaService.ImprimirRecetaAsync(id);
      return Ok(result);
    }
  }
}