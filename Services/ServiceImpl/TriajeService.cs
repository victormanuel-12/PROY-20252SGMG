using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Repository;
using SGMG.Dtos.Request.Triaje;
using SGMG.Data;
using Microsoft.EntityFrameworkCore;

namespace SGMG.Services.ServiceImpl
{
    public class TriajeService : ITriajeService
    {
        private readonly ITriajeRepository _triajeRepository;
        private readonly ApplicationDbContext _context;

        public TriajeService(ITriajeRepository triajeRepository, ApplicationDbContext context)
        {
            _triajeRepository = triajeRepository;
            _context = context;
        }

        public async Task<GenericResponse<Triaje>> AddTriajeAsync(TriajeRequestDTO triajeRequestDTO)
        {
            try
            {
                if (triajeRequestDTO == null)
                    return new GenericResponse<Triaje>(false, "El triaje no puede ser nulo.");

                var triaje = MapToTriaje(triajeRequestDTO);

                // Registrar el triaje
                await _triajeRepository.AddTriajeAsync(triaje);

                // Actualizar el estado de la cita a "Triada"
                await ActualizarEstadoCita(triajeRequestDTO.IdPaciente);

                return new GenericResponse<Triaje>(true, triaje, "Triaje registrado exitosamente.");
            }
            catch (Exception ex)
            {
                return new GenericResponse<Triaje>(false, $"Error al registrar el triaje: {ex.Message}");
            }
        }

        private async Task ActualizarEstadoCita(int idPaciente)
        {
            try
            {
                Console.WriteLine($"Actualizando cita del paciente: {idPaciente}");

                // Traer todas las citas confirmadas del paciente
                var citas = await _context.Citas
                    .Where(c => c.IdPaciente == idPaciente && c.EstadoCita == "Confirmada")
                    .ToListAsync();

                // Ordenar en memoria (evita error de TimeSpan en SQLite)
                var cita = citas
                    .OrderByDescending(c => c.FechaCita)
                    .ThenByDescending(c => c.HoraCita)
                    .FirstOrDefault();

                if (cita != null)
                {
                    Console.WriteLine($"Cita encontrada ID: {cita.IdCita}, Estado: {cita.EstadoCita}");

                    cita.EstadoCita = "Triada";
                    _context.Entry(cita).Property(c => c.EstadoCita).IsModified = true;

                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Cita ID {cita.IdCita} actualizada a: Triada");
                }
                else
                {
                    Console.WriteLine($"No se encontró cita confirmada para el paciente {idPaciente}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar cita: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
            }
        }

        public async Task<GenericResponse<TriajeResponseDTO>> GetTriajeByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return new GenericResponse<TriajeResponseDTO>(false, "El ID del triaje no es válido.");

                var triaje = await _triajeRepository.GetTriajeByIdAsync(id);

                if (triaje == null)
                    return new GenericResponse<TriajeResponseDTO>(false, "Triaje no encontrado.");

                // Mapear a DTO completo
                var triajeDTO = new TriajeResponseDTO
                {
                    // Datos del triaje
                    IdTriaje = triaje.IdTriage,
                    Temperatura = triaje.Temperatura,
                    PresionArterial = triaje.PresionArterial,
                    Saturacion = triaje.Saturacion,
                    FrecuenciaCardiaca = triaje.FrecuenciaCardiaca,
                    FrecuenciaRespiratoria = triaje.FrecuenciaRespiratoria,
                    Peso = triaje.Peso,
                    Talla = triaje.Talla,
                    PerimetroAbdominal = triaje.PerimetroAbdominal,
                    SuperficieCorporal = triaje.SuperficieCorporal,
                    Imc = triaje.Imc,
                    ClasificacionImc = triaje.ClasificacionImc,
                    RiesgoEnfermedad = triaje.RiesgoEnfermedad,
                    EstadoTriage = triaje.EstadoTriage,
                    FechaTriage = triaje.FechaTriage,
                    HoraTriage = triaje.HoraTriage,
                    Observaciones = triaje.Observaciones,

                    // Datos del paciente (COMPLETOS)
                    IdPaciente = triaje.IdPaciente,
                    NumeroDocumento = triaje.Paciente?.NumeroDocumento ?? "",
                    TipoDocumento = triaje.Paciente?.TipoDocumento ?? "",
                    Nombre = triaje.Paciente?.Nombre ?? "",
                    ApellidoPaterno = triaje.Paciente?.ApellidoPaterno ?? "",
                    ApellidoMaterno = triaje.Paciente?.ApellidoMaterno ?? "",
                    Sexo = triaje.Paciente?.Sexo ?? "",
                    Edad = triaje.Paciente?.Edad ?? 0,
                    NombreCompletoPaciente = triaje.Paciente != null
                        ? $"{triaje.Paciente.ApellidoPaterno} {triaje.Paciente.ApellidoMaterno} {triaje.Paciente.Nombre}".Trim()
                        : ""
                };

                return new GenericResponse<TriajeResponseDTO>(true, triajeDTO, "Triaje obtenido correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTriajeByIdAsync: {ex.Message}");
                return new GenericResponse<TriajeResponseDTO>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<TriajeResponseDTO>> UpdateTriajeAsync(TriajeRequestDTO triajeRequestDTO)
        {
            try
            {
                if (triajeRequestDTO == null || triajeRequestDTO.IdPaciente <= 0)
                    return new GenericResponse<TriajeResponseDTO>(false, "Datos inválidos.");

                // Obtener triaje (incluye Paciente)
                var triaje = await _triajeRepository.GetTriajeByIdAsync(triajeRequestDTO.IdTriaje);
                if (triaje == null)
                    return new GenericResponse<TriajeResponseDTO>(false, "Triaje no encontrado.");

                // Guardar referencia al paciente
                var paciente = triaje.Paciente;

                // Actualizar solo campos del triaje
                triaje = MapToTriaje(triajeRequestDTO, triaje);
                await _triajeRepository.UpdateTriajeAsync(triaje);

                // Mapear a DTO (sin consulta adicional)
                var triajeDTO = new TriajeResponseDTO
                {
                    // Datos del triaje (actualizados)
                    IdTriaje = triaje.IdTriage,
                    Temperatura = triaje.Temperatura,
                    PresionArterial = triaje.PresionArterial,
                    Saturacion = triaje.Saturacion,
                    FrecuenciaCardiaca = triaje.FrecuenciaCardiaca,
                    FrecuenciaRespiratoria = triaje.FrecuenciaRespiratoria,
                    Peso = triaje.Peso,
                    Talla = triaje.Talla,
                    PerimetroAbdominal = triaje.PerimetroAbdominal,
                    SuperficieCorporal = triaje.SuperficieCorporal,
                    Imc = triaje.Imc,
                    ClasificacionImc = triaje.ClasificacionImc,
                    RiesgoEnfermedad = triaje.RiesgoEnfermedad,
                    EstadoTriage = triaje.EstadoTriage,
                    FechaTriage = triaje.FechaTriage,
                    HoraTriage = triaje.HoraTriage,
                    Observaciones = triaje.Observaciones,

                    // Datos del paciente (de memoria, no de BD)
                    IdPaciente = triaje.IdPaciente,
                    NumeroDocumento = paciente?.NumeroDocumento ?? "",
                    TipoDocumento = paciente?.TipoDocumento ?? "",
                    Nombre = paciente?.Nombre ?? "",
                    ApellidoPaterno = paciente?.ApellidoPaterno ?? "",
                    ApellidoMaterno = paciente?.ApellidoMaterno ?? "",
                    Sexo = paciente?.Sexo ?? "",
                    Edad = paciente?.Edad ?? 0
                };

                return new GenericResponse<TriajeResponseDTO>(true, triajeDTO, "Triaje actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateTriajeAsync: {ex.Message}");
                return new GenericResponse<TriajeResponseDTO>(false, $"Error: {ex.Message}");
            }
        }


        public async Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> GetAllTriajesAsync()
        {
            try
            {
                Console.WriteLine("Obteniendo todos los triajes...");

                // Traer todos los triajes
                var triajes = await _triajeRepository.GetAllTriajesAsync();

                if (triajes == null || !triajes.Any())
                {
                    return new GenericResponse<IEnumerable<TriajeResponseDTO>>(
                        true,
                        new List<TriajeResponseDTO>(),
                        "No hay triajes registrados."
                    );
                }

                // Obtener IDs únicos de pacientes
                var idsPacientes = triajes.Select(t => t.IdPaciente).Distinct().ToList();

                // Traer TODAS las citas "Triadas" de esos pacientes en UNA sola consulta
                var citasTriadas = await _context.Citas
                    .Include(c => c.Medico)
                    .Where(c => idsPacientes.Contains(c.IdPaciente) && c.EstadoCita == "Triada")
                    .ToListAsync();

                // Agrupar citas por IdPaciente para búsqueda rápida
                var citasPorPaciente = citasTriadas
                    .GroupBy(c => c.IdPaciente)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(c => c.FechaCita)
                               .ThenByDescending(c => c.HoraCita)
                               .First()
                    );

                // Mapear a DTO
                var triajesDTO = triajes.Select(t =>
                {
                    // Buscar la cita del paciente (ya está en memoria)
                    citasPorPaciente.TryGetValue(t.IdPaciente, out var cita);

                    return new TriajeResponseDTO
                    {
                        // Datos del triaje
                        IdTriaje = t.IdTriage,
                        Temperatura = t.Temperatura,
                        PresionArterial = t.PresionArterial,
                        Saturacion = t.Saturacion,
                        FrecuenciaCardiaca = t.FrecuenciaCardiaca,
                        FrecuenciaRespiratoria = t.FrecuenciaRespiratoria,
                        Peso = t.Peso,
                        Talla = t.Talla,
                        PerimetroAbdominal = t.PerimetroAbdominal,
                        SuperficieCorporal = t.SuperficieCorporal,
                        Imc = t.Imc,
                        ClasificacionImc = t.ClasificacionImc,
                        RiesgoEnfermedad = t.RiesgoEnfermedad,
                        EstadoTriage = t.EstadoTriage,
                        FechaTriage = t.FechaTriage,
                        HoraTriage = t.HoraTriage,
                        Observaciones = t.Observaciones,

                        // Datos del paciente
                        IdPaciente = t.IdPaciente,
                        NumeroDocumento = t.Paciente?.NumeroDocumento ?? "",
                        TipoDocumento = t.Paciente?.TipoDocumento ?? "",
                        Nombre = t.Paciente?.Nombre ?? "",
                        ApellidoPaterno = t.Paciente?.ApellidoPaterno ?? "",
                        ApellidoMaterno = t.Paciente?.ApellidoMaterno ?? "",
                        Sexo = t.Paciente?.Sexo ?? "",
                        Edad = t.Paciente?.Edad ?? 0,
                        NombreCompletoPaciente = t.Paciente != null
                            ? $"{t.Paciente.ApellidoPaterno} {t.Paciente.ApellidoMaterno} {t.Paciente.Nombre}".Trim()
                            : "",

                        // Datos de la cita
                        Consultorio = cita?.Consultorio ?? "N/A",
                        HoraCita = cita?.HoraCita.ToString(@"hh\:mm") ?? "00:00",
                        FechaCita = cita?.FechaCita,
                        NombreCompletoMedico = cita?.Medico != null
                            ? $"{cita.Medico.ApellidoPaterno} {cita.Medico.ApellidoMaterno} {cita.Medico.Nombre}".Trim()
                            : "N/A"
                    };
                }).ToList();

                Console.WriteLine($"Total triajes procesados: {triajesDTO.Count}");

                return new GenericResponse<IEnumerable<TriajeResponseDTO>>(
                    true,
                    triajesDTO,
                    "Triajes obtenidos correctamente."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllTriajesAsync: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return new GenericResponse<IEnumerable<TriajeResponseDTO>>(
                    false,
                    $"Error al obtener triajes: {ex.Message}"
                );
            }
        }

        private Triaje MapToTriaje(TriajeRequestDTO dto, Triaje? triaje = null)
        {
            if (triaje == null)
                triaje = new Triaje();

            triaje.IdPaciente = dto.IdPaciente;
            triaje.Temperatura = dto.Temperatura;
            triaje.PresionArterial = dto.PresionArterial;
            triaje.Saturacion = dto.Saturacion;
            triaje.FrecuenciaCardiaca = dto.FrecuenciaCardiaca;
            triaje.FrecuenciaRespiratoria = dto.FrecuenciaRespiratoria;
            triaje.Peso = dto.Peso;
            triaje.Talla = dto.Talla;
            triaje.PerimetroAbdominal = dto.PerimAbdominal;
            triaje.SuperficieCorporal = dto.SuperficieCorporal;
            triaje.Imc = dto.Imc;
            triaje.ClasificacionImc = dto.ClasificacionImc;
            triaje.RiesgoEnfermedad = dto.RiesgoEnfermedad;
            triaje.EstadoTriage = dto.EstadoTriage;
            triaje.Observaciones = dto.Observaciones;

            return triaje;
        }

        public async Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> BuscarTriajesAsync(string? tipoDoc, string? numeroDoc, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                Console.WriteLine($"Buscando triajes: TipoDoc={tipoDoc}, NumDoc={numeroDoc}, FechaInicio={fechaInicio}, FechaFin={fechaFin}");

                // Construir query base
                var query = _context.Triages
                    .Include(t => t.Paciente)
                    .AsQueryable();

                // Filtrar por documento si se proporcionó
                if (!string.IsNullOrWhiteSpace(tipoDoc) && !string.IsNullOrWhiteSpace(numeroDoc))
                {
                    query = query.Where(t =>
                        t.Paciente.TipoDocumento == tipoDoc &&
                        t.Paciente.NumeroDocumento == numeroDoc);
                }

                // Filtrar por rango de fechas
                if (fechaInicio.HasValue)
                {
                    query = query.Where(t => t.FechaTriage >= fechaInicio.Value.Date);
                }

                if (fechaFin.HasValue)
                {
                    var fechaFinInclusiva = fechaFin.Value.Date.AddDays(1);
                    query = query.Where(t => t.FechaTriage < fechaFinInclusiva);
                }

                var triajes = await query.ToListAsync();

                if (!triajes.Any())
                {
                    return new GenericResponse<IEnumerable<TriajeResponseDTO>>(
                        true,
                        new List<TriajeResponseDTO>(),
                        "No se encontraron triajes con los criterios especificados");
                }

                // Ordenar en memoria
                triajes = triajes
                    .OrderByDescending(t => t.FechaTriage)
                    .ThenByDescending(t => t.HoraTriage)
                    .ToList();

                // Obtener IDs de pacientes
                var idsPacientes = triajes.Select(t => t.IdPaciente).Distinct().ToList();

                // Traer citas triadas
                var citasTriadas = await _context.Citas
                    .Include(c => c.Medico)
                    .Where(c => idsPacientes.Contains(c.IdPaciente) && c.EstadoCita == "Triada")
                    .ToListAsync();

                var citasPorPaciente = citasTriadas
                    .GroupBy(c => c.IdPaciente)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(c => c.FechaCita)
                               .ThenByDescending(c => c.HoraCita)
                               .First()
                    );

                // Mapear a DTO
                var triajesDTO = triajes.Select(t =>
                {
                    citasPorPaciente.TryGetValue(t.IdPaciente, out var cita);

                    return new TriajeResponseDTO
                    {
                        IdTriaje = t.IdTriage,
                        Temperatura = t.Temperatura,
                        PresionArterial = t.PresionArterial,
                        Saturacion = t.Saturacion,
                        FrecuenciaCardiaca = t.FrecuenciaCardiaca,
                        FrecuenciaRespiratoria = t.FrecuenciaRespiratoria,
                        Peso = t.Peso,
                        Talla = t.Talla,
                        PerimetroAbdominal = t.PerimetroAbdominal,
                        SuperficieCorporal = t.SuperficieCorporal,
                        Imc = t.Imc,
                        ClasificacionImc = t.ClasificacionImc,
                        RiesgoEnfermedad = t.RiesgoEnfermedad,
                        EstadoTriage = t.EstadoTriage,
                        FechaTriage = t.FechaTriage,
                        HoraTriage = t.HoraTriage,
                        Observaciones = t.Observaciones,

                        IdPaciente = t.IdPaciente,
                        NumeroDocumento = t.Paciente?.NumeroDocumento ?? "",
                        TipoDocumento = t.Paciente?.TipoDocumento ?? "",
                        Nombre = t.Paciente?.Nombre ?? "",
                        ApellidoPaterno = t.Paciente?.ApellidoPaterno ?? "",
                        ApellidoMaterno = t.Paciente?.ApellidoMaterno ?? "",
                        Sexo = t.Paciente?.Sexo ?? "",
                        Edad = t.Paciente?.Edad ?? 0,
                        NombreCompletoPaciente = t.Paciente != null
                            ? $"{t.Paciente.ApellidoPaterno} {t.Paciente.ApellidoMaterno} {t.Paciente.Nombre}".Trim()
                            : "",

                        Consultorio = cita?.Consultorio ?? "N/A",
                        HoraCita = cita?.HoraCita.ToString(@"hh\:mm") ?? "00:00",
                        FechaCita = cita?.FechaCita,
                        NombreCompletoMedico = cita?.Medico != null
                            ? $"{cita.Medico.ApellidoPaterno} {cita.Medico.ApellidoMaterno} {cita.Medico.Nombre}".Trim()
                            : "N/A"
                    };
                }).ToList();

                Console.WriteLine($"Triajes encontrados: {triajesDTO.Count}");

                return new GenericResponse<IEnumerable<TriajeResponseDTO>>(
                    true,
                    triajesDTO,
                    $"Se encontraron {triajesDTO.Count} triajes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en BuscarTriajesAsync: {ex.Message}");
                return new GenericResponse<IEnumerable<TriajeResponseDTO>>(false, $"Error: {ex.Message}");
            }
        }
    }
}

