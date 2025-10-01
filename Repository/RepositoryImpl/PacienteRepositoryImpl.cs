using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGMG.Models;
using SGMG.Data;

namespace SGMG.Repository.RepositoryImpl
{
  public class PacienteRepositoryImpl : IPacienteRepository
  {
    private readonly ApplicationDbContext _context;

    public PacienteRepositoryImpl(ApplicationDbContext context)
    {
      _context = context;
    }

    // Obtener todos los pacientes
    public async Task<IEnumerable<Paciente>> GetAllPacientesAsync()
    {
      return await _context.Pacientes.ToListAsync();
    }

    // Obtener paciente por Id
    public async Task<Paciente?> GetPacienteByIdAsync(int id)
    {
      return await _context.Pacientes.FindAsync(id);
    }

    // Agregar paciente
    public async Task AddPacienteAsync(Paciente paciente)
    {
      await _context.Pacientes.AddAsync(paciente);
      await _context.SaveChangesAsync();
    }

    // Actualizar paciente
    public async Task UpdatePacienteAsync(Paciente paciente)
    {
      _context.Pacientes.Update(paciente);
      await _context.SaveChangesAsync();
    }

    // Eliminar paciente
    public async Task DeletePacienteAsync(int id)
    {
      var paciente = await _context.Pacientes.FindAsync(id);
      if (paciente != null)
      {
        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
      }
    }
  }
}