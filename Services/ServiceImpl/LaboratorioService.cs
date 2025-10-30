using SGMG.Data;
using SGMG.Dtos.Request;
using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Repository;
using Microsoft.EntityFrameworkCore;

namespace SGMG.Services.ServiceImpl
{
    public class LaboratorioService : ILaboratorioService
    {
        private readonly IOrdenLaboratorioRepository _laboratorioRepository;
        private readonly ApplicationDbContext _context;

        public LaboratorioService(
            IOrdenLaboratorioRepository laboratorioRepository,
            ApplicationDbContext context)
        {
            _laboratorioRepository = laboratorioRepository;
            _context = context;
        }

        public async Task<GenericResponse<LaboratorioHistorialDTO>> GetHistorialLaboratorioAsync(int idPaciente)
        {
            try
            {
                // Obtener información del paciente
                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente);

                if (paciente == null)
                    return new GenericResponse<LaboratorioHistorialDTO>(false, "Paciente no encontrado");

                // Obtener historia clínica
                var historiaClinica = await _context.HistoriasClinicas
                    .FirstOrDefaultAsync(h => h.IdPaciente == idPaciente);

                // Obtener órdenes
                var ordenes = await _laboratorioRepository.GetOrdenesByPacienteAsync(idPaciente);

                // Calcular edad
                int edad = paciente.Edad;
                if (historiaClinica?.FechaNacimiento != null)
                {
                    var today = DateTime.Today;
                    edad = today.Year - historiaClinica.FechaNacimiento.Year;
                    if (historiaClinica.FechaNacimiento.Date > today.AddYears(-edad)) edad--;
                }

                // Mapear a DTO
                var historialDTO = new LaboratorioHistorialDTO
                {
                    Paciente = new PacienteInfoDTO
                    {
                        Dni = paciente.NumeroDocumento,
                        NombreCompleto = $"{paciente.Nombre} {paciente.ApellidoPaterno} {paciente.ApellidoMaterno}".Trim(),
                        Sexo = paciente.Sexo == "M" ? "Masculino" : "Femenino",
                        Edad = edad,
                        Seguro = historiaClinica?.TipoSeguro ?? "No registrado"
                    },
                    Ordenes = ordenes.Select(o => new OrdenLaboratorioResponseDTO
                    {
                        IdOrden = o.IdOrden,
                        NumeroOrden = o.NumeroOrden,
                        TipoExamen = o.TipoExamen,
                        NombreCompletoPaciente = $"{paciente.Nombre} {paciente.ApellidoPaterno} {paciente.ApellidoMaterno}".Trim(),
                        FechaSolicitud = o.FechaSolicitud,
                        ObservacionesAdicionales = o.ObservacionesAdicionales,
                        Resultados = o.Resultados,
                        Estado = o.Estado,
                        FechaResultado = o.FechaResultado
                    }).ToList()
                };

                return new GenericResponse<LaboratorioHistorialDTO>(
                    true,
                    historialDTO,
                    "Historial obtenido correctamente"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new GenericResponse<LaboratorioHistorialDTO>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<OrdenLaboratorioResponseDTO>> CrearOrdenAsync(OrdenLaboratorioRequestDTO dto)
        {
            try
            {
                var numeroOrden = await _laboratorioRepository.GenerarNumeroOrdenAsync();

                var orden = new OrdenLaboratorio
                {
                    IdPaciente = dto.IdPaciente,
                    IdMedico = dto.IdMedico,
                    NumeroOrden = numeroOrden,
                    TipoExamen = dto.TipoExamen,
                    ObservacionesAdicionales = dto.ObservacionesAdicionales,
                    Estado = "Pendiente" // ⭐ Estado por defecto
                };

                await _laboratorioRepository.AddOrdenAsync(orden);

                var paciente = await _context.Pacientes.FindAsync(dto.IdPaciente);

                var responseDTO = new OrdenLaboratorioResponseDTO
                {
                    IdOrden = orden.IdOrden,
                    NumeroOrden = orden.NumeroOrden,
                    TipoExamen = orden.TipoExamen,
                    NombreCompletoPaciente = paciente != null
                        ? $"{paciente.Nombre} {paciente.ApellidoPaterno} {paciente.ApellidoMaterno}".Trim()
                        : "",
                    FechaSolicitud = orden.FechaSolicitud,
                    ObservacionesAdicionales = orden.ObservacionesAdicionales,
                    Estado = orden.Estado
                };

                return new GenericResponse<OrdenLaboratorioResponseDTO>(
                    true,
                    responseDTO,
                    "Orden creada exitosamente"
                );
            }
            catch (Exception ex)
            {
                return new GenericResponse<OrdenLaboratorioResponseDTO>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<OrdenLaboratorioResponseDTO>> ActualizarOrdenAsync(OrdenLaboratorioRequestDTO dto)
        {
            try
            {
                var orden = await _laboratorioRepository.GetOrdenByIdAsync(dto.IdOrden);
                if (orden == null)
                    return new GenericResponse<OrdenLaboratorioResponseDTO>(false, "Orden no encontrada");

                orden.TipoExamen = dto.TipoExamen;
                orden.ObservacionesAdicionales = dto.ObservacionesAdicionales;
                orden.Resultados = dto.Resultados;
                orden.Estado = dto.Estado;

                if (dto.Estado == "Realizado" && !orden.FechaResultado.HasValue)
                {
                    orden.FechaResultado = DateTime.UtcNow;
                }

                await _laboratorioRepository.UpdateOrdenAsync(orden);

                var responseDTO = new OrdenLaboratorioResponseDTO
                {
                    IdOrden = orden.IdOrden,
                    NumeroOrden = orden.NumeroOrden,
                    TipoExamen = orden.TipoExamen,
                    NombreCompletoPaciente = orden.Paciente != null
                        ? $"{orden.Paciente.Nombre} {orden.Paciente.ApellidoPaterno} {orden.Paciente.ApellidoMaterno}".Trim()
                        : "",
                    FechaSolicitud = orden.FechaSolicitud,
                    ObservacionesAdicionales = orden.ObservacionesAdicionales,
                    Resultados = orden.Resultados,
                    Estado = orden.Estado,
                    FechaResultado = orden.FechaResultado
                };

                return new GenericResponse<OrdenLaboratorioResponseDTO>(
                    true,
                    responseDTO,
                    "Orden actualizada exitosamente"
                );
            }
            catch (Exception ex)
            {
                return new GenericResponse<OrdenLaboratorioResponseDTO>(false, $"Error: {ex.Message}");
            }
        }

        public async Task<GenericResponse<OrdenLaboratorioResponseDTO>> GetOrdenByIdAsync(int idOrden)
        {
            try
            {
                var orden = await _laboratorioRepository.GetOrdenByIdAsync(idOrden);
                if (orden == null)
                    return new GenericResponse<OrdenLaboratorioResponseDTO>(false, "Orden no encontrada");

                var responseDTO = new OrdenLaboratorioResponseDTO
                {
                    IdOrden = orden.IdOrden,
                    NumeroOrden = orden.NumeroOrden,
                    TipoExamen = orden.TipoExamen,
                    NombreCompletoPaciente = orden.Paciente != null
                        ? $"{orden.Paciente.Nombre} {orden.Paciente.ApellidoPaterno} {orden.Paciente.ApellidoMaterno}".Trim()
                        : "",
                    FechaSolicitud = orden.FechaSolicitud,
                    ObservacionesAdicionales = orden.ObservacionesAdicionales,
                    Resultados = orden.Resultados,
                    Estado = orden.Estado,
                    FechaResultado = orden.FechaResultado
                };

                return new GenericResponse<OrdenLaboratorioResponseDTO>(
                    true,
                    responseDTO,
                    "Orden obtenida correctamente"
                );
            }
            catch (Exception ex)
            {
                return new GenericResponse<OrdenLaboratorioResponseDTO>(false, $"Error: {ex.Message}");
            }
        }
    }
}