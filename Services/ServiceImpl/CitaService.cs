using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Repository;

namespace SGMG.Services.ServiceImpl
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly ILogger<CitaService> _logger;

        public CitaService(ICitaRepository citaRepository, ILogger<CitaService> logger)
        {
            _citaRepository = citaRepository;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasPendientesAsync()
        {
            try
            {
                var citas = await _citaRepository.GetCitasPendientesAsync();
                
                if (citas == null || !citas.Any())
                {
                    return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, new List<CitaResponseDTO>(), "No hay citas pendientes.");
                }

                var citasDTO = citas.Select(MapToDTO).ToList();
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, citasDTO, "Citas pendientes obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(false, $"Error al obtener citas pendientes: {ex.Message}");
            }
        }

        public async Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasFueraHorarioAsync()
        {
            try
            {
                var citas = await _citaRepository.GetCitasFueraHorarioAsync();
                
                if (citas == null || !citas.Any())
                {
                    return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, new List<CitaResponseDTO>(), "No hay citas fuera de horario.");
                }

                var citasDTO = citas.Select(MapToDTO).ToList();
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, citasDTO, "Citas fuera de horario obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetAllCitasAsync()
        {
            try
            {
                var citas = await _citaRepository.GetAllCitasAsync();
                
                if (citas == null || !citas.Any())
                {
                    return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, new List<CitaResponseDTO>(), "No se encontraron citas.");
                }

                var citasDTO = citas.Select(MapToDTO).ToList();
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(true, citasDTO, "Citas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new GenericResponse<IEnumerable<CitaResponseDTO>>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<CitaResponseDTO>> GetCitaByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return new GenericResponse<CitaResponseDTO>(false, "El ID de la cita no es válido.");

                var cita = await _citaRepository.GetCitaByIdAsync(id);

                if (cita == null)
                    return new GenericResponse<CitaResponseDTO>(false, "Cita no encontrada.");

                var citaDTO = MapToDTO(cita);
                return new GenericResponse<CitaResponseDTO>(true, citaDTO, "Cita obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return new GenericResponse<CitaResponseDTO>(false, $"Error: {ex.Message}");
            }
        }

        private CitaResponseDTO MapToDTO(Cita cita)
        {
            return new CitaResponseDTO
            {
                IdCita = cita.IdCita,
                Especialidad = cita.Especialidad,
                FechaCita = cita.FechaCita,
                HoraCita = cita.HoraCita,
                Consultorio = cita.Consultorio,
                EstadoCita = cita.EstadoCita,
                IdPaciente = cita.IdPaciente,
                NumeroDocumento = cita.Paciente?.NumeroDocumento ?? "",
                NombreCompletoPaciente = $"{cita.Paciente?.ApellidoPaterno ?? ""} {cita.Paciente?.ApellidoMaterno ?? ""} {cita.Paciente?.Nombre ?? ""}".Trim(),
                IdMedico = cita.IdMedico,
                NombreCompletoMedico = $"{cita.Medico?.Nombre ?? ""} {cita.Medico?.ApellidoPaterno ?? ""}".Trim()
            };
        }
    }
}