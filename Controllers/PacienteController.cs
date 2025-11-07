using Microsoft.AspNetCore.Mvc;
using SGMG.Models;
using SGMG.Services;
using SGMG.Dtos.Response;
using SGMG.Dtos.Request.Paciente;
using PROY_20252SGMG.Dtos.Response;
using SGMG.Data;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest.Api.V2010.Account;
using Twilio;
using Twilio.Types;


namespace SGMG.Controllers
{
  [Route("[controller]")]
  public class PacienteController : Controller
  {
    private readonly ILogger<PacienteController> _logger;
    private readonly IPacienteService _pacienteService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public PacienteController(
        ILogger<PacienteController> logger,
        IPacienteService pacienteService,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
      _logger = logger;
      _pacienteService = pacienteService;
      _context = context;
      _configuration = configuration;
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
    [HttpPost]
    [Route("/pacientes/enviar-recordatorio/{idCita}")]
    public async Task<IActionResult> EnviarRecordatorioCita(int idCita)
    {
      try
      {
        _logger.LogInformation($"=== INICIO ENVÍO DE RECORDATORIO - Cita ID: {idCita} ===");

        // ============================================
        // PASO 1: CONSULTAR LA CITA EN LA BASE DE DATOS
        // ============================================
        _logger.LogInformation("Consultando información de la cita en la base de datos...");

        var cita = await _context.Citas
            .Include(c => c.Paciente)      // Incluir datos del paciente
            .Include(c => c.Medico)        // Incluir datos del médico
            .Where(c => c.IdCita == idCita)
            .FirstOrDefaultAsync();

        // Validar que la cita existe
        if (cita == null)
        {
          _logger.LogWarning($"No se encontró la cita con ID: {idCita}");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = $"No se encontró la cita con ID: {idCita}"
          });
        }

        _logger.LogInformation($"Cita encontrada: ID {cita.IdCita}, Paciente: {cita.Paciente?.Nombre}");

        // ============================================
        // PASO 2: VALIDAR DATOS DEL PACIENTE
        // ============================================
        if (cita.Paciente == null)
        {
          _logger.LogError($"La cita {idCita} no tiene un paciente asociado");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = "La cita no tiene un paciente asociado."
          });
        }

        // Validar que el paciente tenga teléfono
        if (string.IsNullOrWhiteSpace(cita.Paciente.Telefono))
        {
          _logger.LogWarning($"El paciente {cita.Paciente.IdPaciente} no tiene teléfono registrado");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = "El paciente no tiene un número de teléfono registrado en el sistema."
          });
        }

        _logger.LogInformation($"Teléfono del paciente: {cita.Paciente.Telefono}");

        // ============================================
        // PASO 3: VALIDAR DATOS DEL MÉDICO
        // ============================================
        if (cita.Medico == null)
        {
          _logger.LogError($"La cita {idCita} no tiene un médico asociado");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = "La cita no tiene un médico asociado."
          });
        }

        // ============================================
        // PASO 4: EXTRAER Y FORMATEAR DATOS DE LA BD
        // ============================================
        _logger.LogInformation("Extrayendo y formateando datos de la cita...");

        // Nombre completo del paciente
        string nombrePaciente = $"{cita.Paciente.Nombre} {cita.Paciente.ApellidoPaterno} {cita.Paciente.ApellidoMaterno}".Trim();

        // Nombre completo del médico
        string nombreMedico = $"Dr(a). {cita.Medico.Nombre} {cita.Medico.ApellidoPaterno} {cita.Medico.ApellidoMaterno}".Trim();

        // Fecha formateada (dd/MM/yyyy)
        string fechaCita = cita.FechaCita.ToString("dd/MM/yyyy");

        // Hora formateada (HH:mm)
        string horaCita = cita.HoraCita.ToString(@"hh\:mm");

        // Consultorio
        string consultorio = string.IsNullOrWhiteSpace(cita.Consultorio) ? "Por asignar" : cita.Consultorio;

        // Especialidad (siempre Medicina General)
        string especialidad = "Medicina General";

        _logger.LogInformation($"Datos extraídos - Fecha: {fechaCita}, Hora: {horaCita}, Consultorio: {consultorio}");

        // ============================================
        // PASO 5: FORMATEAR NÚMERO DE TELÉFONO
        // ============================================
        string telefonoOriginal = cita.Paciente.Telefono;
        string telefonoFormateado = FormatearNumeroParaWhatsApp(telefonoOriginal);

        _logger.LogInformation($"Teléfono formateado: {telefonoOriginal} -> {telefonoFormateado}");

        // ============================================
        // PASO 6: CONSTRUIR EL MENSAJE DE WHATSAPP
        // ============================================
        string mensaje = $@"🏥 *RECORDATORIO DE CITA MÉDICA*

Estimado(a) *{nombrePaciente}*,

Le recordamos que tiene una cita programada:

📅 *Fecha:* {fechaCita}
🕐 *Hora:* {horaCita}
👨‍⚕️ *Médico:* {nombreMedico}
🏢 *Consultorio:* {consultorio}
🩺 *Especialidad:* {especialidad}

✅ Por favor, llegue 10 minutos antes de su cita.

⚠️ Si no puede asistir, comuníquese con nosotros para reprogramar.

¡Le esperamos! 💙";

        _logger.LogInformation("Mensaje construido exitosamente");
        _logger.LogInformation($"Contenido del mensaje:\n{mensaje}");

        // ============================================
        // PASO 7: OBTENER CREDENCIALES DE TWILIO
        // ============================================
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];
        var whatsappFrom = _configuration["Twilio:WhatsAppFrom"];

        if (string.IsNullOrWhiteSpace(accountSid) || string.IsNullOrWhiteSpace(authToken))
        {
          _logger.LogError("Credenciales de Twilio no configuradas en appsettings.json");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = "Error de configuración: Credenciales de Twilio no encontradas."
          });
        }

        _logger.LogInformation($"Credenciales de Twilio obtenidas. Account SID: {accountSid?.Substring(0, 10)}...");

        // ============================================
        // PASO 8: INICIALIZAR TWILIO CLIENT
        // ============================================
        _logger.LogInformation("Inicializando cliente de Twilio...");
        TwilioClient.Init(accountSid, authToken);

        // ============================================
        // PASO 9: ENVIAR MENSAJE POR WHATSAPP
        // ============================================
        _logger.LogInformation($"Enviando mensaje de WhatsApp desde {whatsappFrom} hacia {telefonoFormateado}");

        var messageResource = await MessageResource.CreateAsync(
            body: mensaje,
            from: new PhoneNumber(whatsappFrom),
            to: new PhoneNumber($"whatsapp:{telefonoFormateado}")
        );

        // ============================================
        // PASO 10: VALIDAR RESPUESTA DE TWILIO
        // ============================================
        _logger.LogInformation($"Respuesta de Twilio - SID: {messageResource.Sid}, Status: {messageResource.Status}");

        if (messageResource.ErrorCode.HasValue)
        {
          _logger.LogError($"Error de Twilio - Código: {messageResource.ErrorCode}, Mensaje: {messageResource.ErrorMessage}");
          return Ok(new GenericResponse<string>
          {
            Success = false,
            Message = $"Error al enviar mensaje: {messageResource.ErrorMessage}"
          });
        }

        // ============================================
        // PASO 11: RETORNAR RESPUESTA EXITOSA
        // ============================================
        _logger.LogInformation($"=== RECORDATORIO ENVIADO EXITOSAMENTE - SID: {messageResource.Sid} ===");

        return Ok(new GenericResponse<string>
        {
          Success = true,
          Message = $"Recordatorio enviado exitosamente a {nombrePaciente} al número {telefonoOriginal}",
          Data = messageResource.Sid
        });
      }
      catch (Twilio.Exceptions.ApiException twilioEx)
      {
        // Error específico de Twilio
        _logger.LogError(twilioEx, $"Error de API de Twilio al enviar recordatorio de cita {idCita}");
        return Ok(new GenericResponse<string>
        {
          Success = false,
          Message = $"Error de Twilio: {twilioEx.Message}. Código: {twilioEx.Code}"
        });
      }
      catch (DbUpdateException dbEx)
      {
        // Error de base de datos
        _logger.LogError(dbEx, $"Error de base de datos al consultar cita {idCita}");
        return Ok(new GenericResponse<string>
        {
          Success = false,
          Message = "Error al consultar la base de datos."
        });
      }
      catch (Exception ex)
      {
        // Error general
        _logger.LogError(ex, $"Error inesperado al enviar recordatorio de cita {idCita}");
        return Ok(new GenericResponse<string>
        {
          Success = false,
          Message = $"Error inesperado: {ex.Message}"
        });
      }
    }

    /// <summary>
    /// Formatea un número de teléfono para WhatsApp
    /// Agrega el código de país +51 (Perú) si no lo tiene
    /// </summary>
    private string FormatearNumeroParaWhatsApp(string telefono)
    {
      if (string.IsNullOrWhiteSpace(telefono))
      {
        throw new ArgumentException("El número de teléfono no puede estar vacío");
      }

      // Eliminar espacios, guiones, paréntesis y otros caracteres
      string numeroLimpio = new string(telefono.Where(char.IsDigit).ToArray());

      // Si el número tiene 9 dígitos, es un número peruano sin código de país
      if (numeroLimpio.Length == 9)
      {
        return $"+51{numeroLimpio}";
      }

      // Si empieza con 51 pero no tiene el +
      if (numeroLimpio.StartsWith("51") && numeroLimpio.Length == 11)
      {
        return $"+{numeroLimpio}";
      }

      // Si ya tiene el código completo
      if (numeroLimpio.StartsWith("51") || telefono.StartsWith("+51"))
      {
        return telefono.StartsWith("+") ? telefono : $"+{numeroLimpio}";
      }

      // Por defecto, agregar +51 (Perú)
      return $"+51{numeroLimpio}";
    }
  }
}
