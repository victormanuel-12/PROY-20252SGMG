using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGMG.common.exception;
using SGMG.Dtos.Request.Medico;
using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Repository;
using SGMG.Services;


namespace SGMG.Services.ServiceImpl
{
  public class MedicoServiceImpl : IMedicoService
  {
    private readonly IMedicoRepository _medicoRepository;

    public MedicoServiceImpl(IMedicoRepository medicoRepository)
    {
      _medicoRepository = medicoRepository;
    }

    public async Task<GenericResponse<IEnumerable<Medico>>> GetAllMedicosAsync()
    {
      try
      {
        var medicos = await _medicoRepository.GetAllMedicosAsync();
        if (medicos == null || !medicos.Any())
          return new GenericResponse<IEnumerable<Medico>> { Success = false, Message = "No se encontraron médicos.", Data = null };
        return new GenericResponse<IEnumerable<Medico>> { Success = true, Message = "Médicos obtenidos correctamente.", Data = medicos };
      }
      catch (Exception ex)
      {
        throw new GenericException("Error al obtener los médicos.", ex);
      }
    }

    public async Task<GenericResponse<Medico>> GetMedicoByIdAsync(int id)
    {
      try
      {
        if (id <= 0)
          return new GenericResponse<Medico> { Success = false, Message = "El ID del médico no es válido.", Data = null };
        var medico = await _medicoRepository.GetMedicoByIdAsync(id);
        if (medico == null)
          return new GenericResponse<Medico> { Success = false, Message = "Médico no encontrado.", Data = null };
        return new GenericResponse<Medico> { Success = true, Message = "Médico obtenido correctamente.", Data = medico };
      }
      catch (Exception ex)
      {
        throw new GenericException("Error al obtener el médico con ID: " + id, ex);
      }
    }

    public async Task<GenericResponse<Medico>> AddMedicoAsync(MedicoRequestDTO medicoRequestDTO)
    {
      try
      {
        if (medicoRequestDTO == null)
          return new GenericResponse<Medico> { Success = false, Message = "El médico no puede ser nulo.", Data = null };
        var medico = MapToMedico(medicoRequestDTO);
        await _medicoRepository.AddMedicoAsync(medico);
        return new GenericResponse<Medico> { Success = true, Message = "Médico registrado exitosamente.", Data = medico };
      }
      catch (Exception ex)
      {
        return new GenericResponse<Medico> { Success = false, Message = "Error al registrar al medico " + medicoRequestDTO.Nombre + ": " + ex.Message };
      }
    }

    public async Task<GenericResponse<Medico>> UpdateMedicoAsync(MedicoRequestDTO medicoRequestDTO)
    {
      try
      {
        if (medicoRequestDTO == null || medicoRequestDTO.IdMedico <= 0)
          return new GenericResponse<Medico> { Success = false, Message = "El médico no es válido.", Data = null };
        var medicoExistente = await _medicoRepository.GetMedicoByIdAsync(medicoRequestDTO.IdMedico);
        if (medicoExistente == null)
          return new GenericResponse<Medico> { Success = false, Message = "Médico no encontrado.", Data = null };
        var medico = MapToMedico(medicoRequestDTO);
        await _medicoRepository.UpdateMedicoAsync(medico);
        return new GenericResponse<Medico> { Success = true, Message = "Médico actualizado correctamente.", Data = medico };
      }
      catch (Exception ex)
      {
        return new GenericResponse<Medico> { Success = false, Message = "Error al actualizar al medico " + medicoRequestDTO.Nombre + ": " + ex.Message };
      }
    }

    public async Task<GenericResponse<Medico>> DeleteMedicoAsync(int id)
    {
      try
      {
        if (id <= 0)
          return new GenericResponse<Medico> { Success = false, Message = "El ID no es válido.", Data = null };
        var medico = await _medicoRepository.GetMedicoByIdAsync(id);
        if (medico == null)
          return new GenericResponse<Medico> { Success = false, Message = "Médico no encontrado.", Data = null };
        await _medicoRepository.DeleteMedicoAsync(id);
        return new GenericResponse<Medico> { Success = true, Message = "Médico eliminado correctamente.", Data = medico };
      }
      catch (Exception ex)
      {
        return new GenericResponse<Medico> { Success = false, Message = "Error al eliminar al medico con ID: " + id + ": " + ex.Message };
      }
    }

    private Medico MapToMedico(MedicoRequestDTO dto)
    {
      var medico = new Medico
      {
        NumeroDni = dto.NumeroDni,
        Nombre = dto.Nombre,
        ApellidoPaterno = dto.ApellidoPaterno,
        Sexo = dto.Sexo,
        FechaNacimiento = dto.FechaNacimiento,
        CorreoElectronico = dto.CorreoElectronico,
        EstadoLaboral = dto.EstadoLaboral,
        FechaIngreso = dto.FechaIngreso,
        Turno = dto.Turno,
        AreaServicio = dto.AreaServicio,
        CargoMedico = dto.CargoMedico,
        NumeroColegiatura = dto.NumeroColegiatura,
        TipoMedico = dto.TipoMedico,
      };
      if (dto.ApellidoMaterno != null)
        medico.ApellidoMaterno = dto.ApellidoMaterno;
      if (dto.Direccion != null)
        medico.Direccion = dto.Direccion;
      if (dto.Telefono != null)
        medico.Telefono = dto.Telefono;
      if (dto.IdConsultorio != null)
        medico.IdConsultorio = dto.IdConsultorio;
      return medico;
    }
    private MedicoRequestDTO MapToMedicoRequestDTO(Medico medico)
    {
      return new MedicoRequestDTO
      {
        NumeroDni = medico.NumeroDni,
        Nombre = medico.Nombre,
        ApellidoPaterno = medico.ApellidoPaterno,
        ApellidoMaterno = string.IsNullOrEmpty(medico.ApellidoMaterno) ? null : medico.ApellidoMaterno,
        Sexo = medico.Sexo,
        FechaNacimiento = medico.FechaNacimiento,
        Direccion = string.IsNullOrEmpty(medico.Direccion) ? null : medico.Direccion,
        Telefono = string.IsNullOrEmpty(medico.Telefono) ? null : medico.Telefono,
        CorreoElectronico = medico.CorreoElectronico,
        EstadoLaboral = medico.EstadoLaboral,
        FechaIngreso = medico.FechaIngreso,
        Turno = medico.Turno,
        AreaServicio = medico.AreaServicio,
        CargoMedico = medico.CargoMedico,
        NumeroColegiatura = medico.NumeroColegiatura,
        TipoMedico = medico.TipoMedico,
        IdConsultorio = medico.IdConsultorio
      };
    }
  }
}