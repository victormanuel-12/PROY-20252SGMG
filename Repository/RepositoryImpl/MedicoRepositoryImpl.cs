using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGMG.Models;
using SGMG.Repository;
using SGMG.Data;

namespace SGMG.Repository.RepositoryImpl
{
  public class MedicoRepositoryImpl : IMedicoRepository
  {
    private readonly ApplicationDbContext _context;

    // Inyección de dependencias (el DbContext llega desde Program.cs)
    public MedicoRepositoryImpl(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Medico>> GetAllMedicosAsync()
    {
      return await _context.Medicos.ToListAsync();
    }

    public async Task<Medico?> GetMedicoByIdAsync(int id)
    {
      return await _context.Medicos.FindAsync(id);
    }

    public async Task AddMedicoAsync(Medico medico)
    {
      await _context.Medicos.AddAsync(medico);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateMedicoAsync(Medico medico)
    {
      _context.Medicos.Update(medico);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteMedicoAsync(int id)
    {
      var medico = await _context.Medicos.FindAsync(id);
      if (medico != null)
      {
        _context.Medicos.Remove(medico);
        await _context.SaveChangesAsync();
      }
    }
  }
}