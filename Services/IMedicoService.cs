using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGMG.Dtos.Request.Medico;
using SGMG.Dtos.Response;
using SGMG.Models;

namespace SGMG.Services
{
  public interface IMedicoService
  {
    Task<GenericResponse<IEnumerable<Medico>>> GetAllMedicosAsync();
    Task<GenericResponse<Medico>> GetMedicoByIdAsync(int id);
    Task<GenericResponse<Medico>> AddMedicoAsync(MedicoRequestDTO medicoRequestDTO);
    Task<GenericResponse<Medico>> UpdateMedicoAsync(MedicoRequestDTO medicoRequestDTO);
    Task<GenericResponse<Medico>> DeleteMedicoAsync(int id);
  }
}