using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Dtos.Request.Triaje;

namespace SGMG.Services
{
    public interface ITriajeService
    {
        Task<GenericResponse<Triaje>> AddTriajeAsync(TriajeRequestDTO triajeRequestDTO);
        Task<GenericResponse<TriajeResponseDTO>> GetTriajeByIdAsync(int id);
        Task<GenericResponse<TriajeResponseDTO>> UpdateTriajeAsync(TriajeRequestDTO triajeRequestDTO);
        Task<GenericResponse<IEnumerable<TriajeResponseDTO>>> GetAllTriajesAsync(); // Método para obtener todos los triajes
    }
}