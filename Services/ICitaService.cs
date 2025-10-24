using SGMG.Dtos.Response;

namespace SGMG.Services
{
    public interface ICitaService
    {
        Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasPendientesAsync();
        Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasFueraHorarioAsync();
        Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetAllCitasAsync();
        Task<GenericResponse<IEnumerable<CitaResponseDTO>>> BuscarCitasPendientesAsync(string? tipoDoc, string? numeroDoc);
        Task<GenericResponse<IEnumerable<CitaResponseDTO>>> BuscarCitasFueraHorarioAsync(string? tipoDoc, string? numeroDoc);
        Task<GenericResponse<CitaResponseDTO>> GetCitaByIdAsync(int id);
    }
}