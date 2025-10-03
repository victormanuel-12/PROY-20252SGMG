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

        var medico = MapToMedicoAdd(medicoRequestDTO);
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
        var medicoExistente = await _medicoRepository.GetMedicoByIdAsync(medicoRequestDTO.IdMedico.Value);
        if (medicoExistente == null)
          return new GenericResponse<Medico> { Success = false, Message = "Médico no encontrado.", Data = null };

        MapToMedico(medicoRequestDTO, medicoExistente);
        await _medicoRepository.UpdateMedicoAsync(medicoExistente);
        return new GenericResponse<Medico> { Success = true, Message = "Médico actualizado correctamente.", Data = medicoExistente };
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

    private void MapToMedico(MedicoRequestDTO dto, Medico medico)
    {
      // Solo actualizamos los campos si el DTO trae valores válidos

      if (!string.IsNullOrWhiteSpace(dto.NumeroDni))
        medico.NumeroDni = dto.NumeroDni;

      if (!string.IsNullOrWhiteSpace(dto.Nombre))
        medico.Nombre = dto.Nombre;

      if (!string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
        medico.ApellidoPaterno = dto.ApellidoPaterno;

      if (!string.IsNullOrWhiteSpace(dto.ApellidoMaterno))
        medico.ApellidoMaterno = dto.ApellidoMaterno;

      if (!string.IsNullOrWhiteSpace(dto.Sexo))
        medico.Sexo = dto.Sexo;

      if (dto.FechaNacimiento != default)
        medico.FechaNacimiento = dto.FechaNacimiento;

      if (!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
        medico.CorreoElectronico = dto.CorreoElectronico;

      if (!string.IsNullOrWhiteSpace(dto.EstadoLaboral))
        medico.EstadoLaboral = dto.EstadoLaboral;

      if (dto.FechaIngreso != default)
        medico.FechaIngreso = dto.FechaIngreso;

      if (!string.IsNullOrWhiteSpace(dto.Turno))
        medico.Turno = dto.Turno;

      if (!string.IsNullOrWhiteSpace(dto.AreaServicio))
        medico.AreaServicio = dto.AreaServicio;

      if (!string.IsNullOrWhiteSpace(dto.CargoMedico))
        medico.CargoMedico = dto.CargoMedico;

      if (!string.IsNullOrWhiteSpace(dto.NumeroColegiatura))
        medico.NumeroColegiatura = dto.NumeroColegiatura;

      if (!string.IsNullOrWhiteSpace(dto.TipoMedico))
        medico.TipoMedico = dto.TipoMedico;

      if (!string.IsNullOrWhiteSpace(dto.Direccion))
        medico.Direccion = dto.Direccion;

      if (!string.IsNullOrWhiteSpace(dto.Telefono))
        medico.Telefono = dto.Telefono;

      if (dto.IdConsultorio.HasValue && dto.IdConsultorio.Value > 0)
        medico.IdConsultorio = dto.IdConsultorio.Value;
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



    private Medico MapToMedicoAdd(MedicoRequestDTO dto)
    {
      var medico = new Medico();

      // Evaluación para cadenas obligatorias
      if (!string.IsNullOrWhiteSpace(dto.NumeroDni))
        medico.NumeroDni = dto.NumeroDni;

      if (!string.IsNullOrWhiteSpace(dto.Nombre))
        medico.Nombre = dto.Nombre;

      if (!string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
        medico.ApellidoPaterno = dto.ApellidoPaterno;

      if (!string.IsNullOrWhiteSpace(dto.ApellidoMaterno))
        medico.ApellidoMaterno = dto.ApellidoMaterno;

      if (!string.IsNullOrWhiteSpace(dto.Sexo))
        medico.Sexo = dto.Sexo;

      if (dto.FechaNacimiento != default)
        medico.FechaNacimiento = dto.FechaNacimiento;

      if (!string.IsNullOrWhiteSpace(dto.CorreoElectronico))
        medico.CorreoElectronico = dto.CorreoElectronico;

      if (!string.IsNullOrWhiteSpace(dto.EstadoLaboral))
        medico.EstadoLaboral = dto.EstadoLaboral;

      if (dto.FechaIngreso != default)
        medico.FechaIngreso = dto.FechaIngreso;

      if (!string.IsNullOrWhiteSpace(dto.Turno))
        medico.Turno = dto.Turno;

      if (!string.IsNullOrWhiteSpace(dto.AreaServicio))
        medico.AreaServicio = dto.AreaServicio;

      if (!string.IsNullOrWhiteSpace(dto.CargoMedico))
        medico.CargoMedico = dto.CargoMedico;

      if (!string.IsNullOrWhiteSpace(dto.NumeroColegiatura))
        medico.NumeroColegiatura = dto.NumeroColegiatura;

      if (!string.IsNullOrWhiteSpace(dto.TipoMedico))
        medico.TipoMedico = dto.TipoMedico;

      // Campos opcionales
      if (!string.IsNullOrWhiteSpace(dto.Direccion))
        medico.Direccion = dto.Direccion;

      if (!string.IsNullOrWhiteSpace(dto.Telefono))
        medico.Telefono = dto.Telefono;

      if (dto.IdConsultorio.HasValue && dto.IdConsultorio.Value > 0)
        medico.IdConsultorio = dto.IdConsultorio.Value;

      return medico;
    }

  }
}