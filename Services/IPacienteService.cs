using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGMG.Dtos.Request.Paciente;
using SGMG.Dtos.Response;
using SGMG.Models;

namespace SGMG.Services
{
  public interface IPacienteService
  {
    Task<GenericResponse<IEnumerable<Paciente>>> GetAllPacientesAsync();
    Task<GenericResponse<Paciente>> SearchPacienteByDocumentoAsync(string tipoDocumento, string numeroDocumento);
    Task<GenericResponse<IEnumerable<CitaResponseDTO>>> GetCitasPendientesByPacienteAsync(int idPaciente);
    Task<GenericResponse<Paciente>> GetPacienteByIdAsync(int id);
    Task<GenericResponse<Paciente>> AddPacienteAsync(PacienteRequestDTO pacienteRequestDTO);
    Task<GenericResponse<Paciente>> UpdatePacienteAsync(PacienteRequestDTO pacienteRequestDTO);
    Task<GenericResponse<Paciente>> DeletePacienteAsync(int id);
  }
}