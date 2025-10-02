using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    private readonly ApplicationDbContext _context;

    public EnfermeriaServiceImpl(IEnfermeriaRepository enfermeriaRepository, ApplicationDbContext context)
    {
      _enfermeriaRepository = enfermeriaRepository;
      _context = context;
    }

    public async Task<GenericResponse<IEnumerable<EnfermeriaResponse>>> GetAllEnfermeriasAsync()
    {
      var enfermerias = await _enfermeriaRepository.GetAllEnfermeriasAsync();

      if (enfermerias == null || !enfermerias.Any())
        return new GenericResponse<IEnumerable<EnfermeriaResponse>>(false, "No se encontraron registros de enfermería.");

      return new GenericResponse<IEnumerable<EnfermeriaResponse>>(true, enfermerias, "Registros de enfermería obtenidos correctamente.");
    }

    public async Task<GenericResponse<Enfermeria>> GetEnfermeriaByIdAsync(int id)
    {
      if (id <= 0)
        return new GenericResponse<Enfermeria>(false, "El ID no es válido.");

      var enfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(id);

      if (enfermeria == null)
        return new GenericResponse<Enfermeria>(false, "Registro de enfermería no encontrado.");

      return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería obtenido correctamente.");
    }

    public async Task<GenericResponse<Enfermeria>> AddEnfermeriaAsync(EnfermeriaRequestDTO enfermeriaRequestDTO)
    {
      if (enfermeriaRequestDTO == null)
        return new GenericResponse<Enfermeria>(false, "El registro de enfermería no puede ser nulo.");

      // ✅ Validar que el Consultorio existe (opcional pero recomendado)
      if (enfermeriaRequestDTO.IdConsultorio > 0)
      {
        var consultorioExiste = await _context.Consultorios
            .AnyAsync(c => c.IdConsultorio == enfermeriaRequestDTO.IdConsultorio);

        if (!consultorioExiste)
          return new GenericResponse<Enfermeria>(false,
              $"No existe un consultorio con ID {enfermeriaRequestDTO.IdConsultorio}. Por favor, verifique el ID.");
      }

      // ✅ Validar que el Personal existe (opcional pero recomendado)
      if (enfermeriaRequestDTO.IdPersonal.HasValue && enfermeriaRequestDTO.IdPersonal.Value > 0)
      {
        var personalExiste = await _context.PersonalTecnicos
            .AnyAsync(p => p.IdPersonal == enfermeriaRequestDTO.IdPersonal.Value);

        if (!personalExiste)
          return new GenericResponse<Enfermeria>(false,
              $"No existe personal con ID {enfermeriaRequestDTO.IdPersonal}. Por favor, verifique el ID.");
      }

      var enfermeria = MapToEnfermeria(enfermeriaRequestDTO);

      // ❌ SIN try-catch - Deja que el GlobalExceptionFilter maneje los errores
      await _enfermeriaRepository.AddEnfermeriaAsync(enfermeria);

      return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería agregado correctamente.");
    }

    public async Task<GenericResponse<Enfermeria>> UpdateEnfermeriaAsync(EnfermeriaRequestDTO enfermeriaRequestDTO)
    {
      if (enfermeriaRequestDTO == null || enfermeriaRequestDTO.IdEnfermeria <= 0)
        return new GenericResponse<Enfermeria> { Success = false, Message = "El registro de enfermería no es válido.", Data = null };

      var enfermeria = MapToEnfermeria(enfermeriaRequestDTO);
      var existingEnfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(enfermeria.IdEnfermeria);

      if (existingEnfermeria == null)
        return new GenericResponse<Enfermeria> { Success = false, Message = "Registro de enfermería no encontrado.", Data = null };

      // ✅ Validaciones opcionales de FK antes de actualizar
      if (enfermeriaRequestDTO.IdConsultorio > 0 && enfermeriaRequestDTO.IdConsultorio != existingEnfermeria.IdConsultorio)
      {
        var consultorioExiste = await _context.Consultorios
            .AnyAsync(c => c.IdConsultorio == enfermeriaRequestDTO.IdConsultorio);

        if (!consultorioExiste)
          return new GenericResponse<Enfermeria>(false,
              $"No existe un consultorio con ID {enfermeriaRequestDTO.IdConsultorio}.");
      }

      UpdateEnfermeriaProperties(existingEnfermeria, enfermeria);

      // ❌ SIN try-catch
      await _enfermeriaRepository.UpdateEnfermeriaAsync(existingEnfermeria);

      return new GenericResponse<Enfermeria> { Success = true, Message = "Registro de enfermería actualizado correctamente.", Data = existingEnfermeria };
    }

    public async Task<GenericResponse<Enfermeria>> DeleteEnfermeriaAsync(int id)
    {
      if (id <= 0)
        return new GenericResponse<Enfermeria>(false, "El ID no es válido.");

      var enfermeria = await _enfermeriaRepository.GetEnfermeriaByIdAsync(id);

      if (enfermeria == null)
        return new GenericResponse<Enfermeria>(false, "Registro de enfermería no encontrado.");

      // ❌ SIN try-catch
      await _enfermeriaRepository.DeleteEnfermeriaAsync(id);

      return new GenericResponse<Enfermeria>(true, enfermeria, "Registro de enfermería eliminado correctamente.");
    }

    private Enfermeria MapToEnfermeria(EnfermeriaRequestDTO dto)
    {
      var enfermeria = new Enfermeria();

      if (dto.IdEnfermeria.HasValue)
        enfermeria.IdEnfermeria = dto.IdEnfermeria.Value;
      if (dto.NumeroColegiaturaEnfermeria != null)
        enfermeria.NumeroColegiaturaEnfermeria = dto.NumeroColegiaturaEnfermeria;
      if (dto.NivelProfesional != null)
        enfermeria.NivelProfesional = dto.NivelProfesional;
      if (dto.IdConsultorio > 0)
        enfermeria.IdConsultorio = dto.IdConsultorio;
      if (dto.IdPersonal.HasValue)
        enfermeria.IdPersonal = dto.IdPersonal.Value;

      return enfermeria;
    }

    private void UpdateEnfermeriaProperties(Enfermeria existing, Enfermeria updated)
    {
      if (!string.IsNullOrEmpty(updated.NumeroColegiaturaEnfermeria))
        existing.NumeroColegiaturaEnfermeria = updated.NumeroColegiaturaEnfermeria;
      if (!string.IsNullOrEmpty(updated.NivelProfesional))
        existing.NivelProfesional = updated.NivelProfesional;
      if (updated.IdConsultorio > 0)
        existing.IdConsultorio = updated.IdConsultorio;
      if (updated.IdPersonal > 0)
        existing.IdPersonal = updated.IdPersonal;
    }
  }
}