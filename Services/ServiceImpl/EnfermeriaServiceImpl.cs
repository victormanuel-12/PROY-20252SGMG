using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SGMG.Dtos.Response;
using SGMG.Models;
using SGMG.Services;
using SGMG.common.exception;
using SGMG.Data;
using SGMG.Repository;
using SGMG.Dtos.Request.Enfermeria;


namespace SGMG.Services.ServiceImpl
{
  public class EnfermeriaServiceImpl : IEnfermeriaService
  {
    private readonly IEnfermeriaRepository _enfermeriaRepository;

    public EnfermeriaServiceImpl(IEnfermeriaRepository enfermeriaRepository)
    {
      _enfermeriaRepository = enfermeriaRepository;
    }

    public async Task<GenericResponse<IEnumerable<EnfermeriaResponse>>> GetAllEnfermeriasAsync()
    {
      try
      {
        var enfermerias = await _enfermeriaRepository.GetAllEnfermeriasAsync();
        if (enfermerias == null || !enfermerias.Any())
          return new GenericResponse<IEnumerable<EnfermeriaResponse>>(false, "No se encontraron registros de enfermería.");
        return new GenericResponse<IEnumerable<EnfermeriaResponse>>(true, enfermerias, "Registros de enfermería obtenidos correctamente.");
      }
      catch (Exception ex)
      {
        throw new GenericException("Error al obtener los registros de enfermería.", ex);
      }
    }

    public async Task<GenericResponse<Enfermeria>> GetEnfermeriaByIdAsync(int id)
    {
      try
      {
        if (id <= 0)
          return new GenericResponse<Enfermeria>(false, "El ID no es válido.");
        var enfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(id);
        if (enfermeria == null)
          return new GenericResponse<Enfermeria>(false, "Registro de enfermería no encontrado.");
        return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería obtenido correctamente.");
      }
      catch (Exception ex)
      {
        return new GenericResponse<Enfermeria> { Success = false, Message = $"Error al obtener el registro de enfermería con ID: {id}", Data = null };
      }
    }

    public async Task<GenericResponse<Enfermeria>> AddEnfermeriaAsync(EnfermeriaRequestDTO enfermeriaRequestDTO)
    {
      try
      {
        if (enfermeriaRequestDTO == null)
          return new GenericResponse<Enfermeria>(false, "El registro de enfermería no puede ser nulo.");
        var enfermeria = MapToEnfermeria(enfermeriaRequestDTO);
        await _enfermeriaRepository.AddEnfermeriaAsync(enfermeria);
        return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería agregado correctamente.");
      }
      catch (Exception ex)
      {
        return new GenericResponse<Enfermeria> { Success = false, Message = "Error al agregar el registro de enfermería: " + ex.Message, Data = null };
      }
    }



    public async Task<GenericResponse<Enfermeria>> UpdateEnfermeriaAsync(EnfermeriaRequestDTO enfermeriaRequestDTO)
    {
      try
      {
        if (enfermeriaRequestDTO == null || enfermeriaRequestDTO.IdEnfermeria <= 0)
          return new GenericResponse<Enfermeria> { Success = false, Message = "El registro de enfermería no es válido.", Data = null };
        var enfermeria = MapToEnfermeria(enfermeriaRequestDTO);
        var existingEnfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(enfermeria.IdEnfermeria);
        if (existingEnfermeria == null)
          return new GenericResponse<Enfermeria> { Success = false, Message = "Registro de enfermería no encontrado.", Data = null };
        await _enfermeriaRepository.UpdateEnfermeriaAsync(existingEnfermeria);
        return new GenericResponse<Enfermeria> { Success = true, Message = "Registro de enfermería actualizado correctamente.", Data = existingEnfermeria };
      }
      catch (Exception ex)
      {
        return new GenericResponse<Enfermeria> { Success = false, Message = "Error al actualizar el registro de enfermería: " + ex.Message, Data = null };
      }
    }

    public async Task<GenericResponse<Enfermeria>> DeleteEnfermeriaAsync(int id)
    {
      try
      {
        if (id <= 0)
          return new GenericResponse<Enfermeria>(false, "El ID no es válido.");
        var enfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(id);
        if (enfermeria == null)
          return new GenericResponse<Enfermeria>(false, "Registro de enfermería no encontrado.");
        await _enfermeriaRepository.DeleteEnfermeriaAsync(id);
        return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería eliminado correctamente.");
      }
      catch (Exception ex)
      {
        return new GenericResponse<Enfermeria>(false, $"Error al eliminar el registro de enfermería con ID {id}: {ex.Message}");
      }
    }

    private Enfermeria MapToEnfermeria(EnfermeriaRequestDTO dto)
    {
      var enfermeria = new Enfermeria();
      if (dto.IdEnfermeria > 0)
        enfermeria.IdEnfermeria = dto.IdEnfermeria;
      if (dto.NumeroColegiaturaEnfermeria != null)
        enfermeria.NumeroColegiaturaEnfermeria = dto.NumeroColegiaturaEnfermeria;
      if (dto.NivelProfesional != null)
        enfermeria.NivelProfesional = dto.NivelProfesional;
      if (dto.IdConsultorio > 0)
        enfermeria.IdConsultorio = dto.IdConsultorio;
      return enfermeria;
    }
  }

}
