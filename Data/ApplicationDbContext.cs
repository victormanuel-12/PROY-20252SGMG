using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGMG.Models;

namespace SGMG.Data;

public class ApplicationDbContext : IdentityDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
  {

  }
  public DbSet<Paciente> Pacientes { get; set; }
  public DbSet<HistoriaClinica> HistoriasClinicas { get; set; }
  public DbSet<Triaje> Triages { get; set; }
  public DbSet<Cita> Citas { get; set; }
  public DbSet<Pago> Pagos { get; set; }
  public DbSet<Medico> Medicos { get; set; }
  public DbSet<DisponibilidadMedico> DisponibilidadesMedicos { get; set; }
  public DbSet<PersonalTecnico> PersonalTecnicos { get; set; }
  public DbSet<Enfermeria> Enfermerias { get; set; }
  public DbSet<DomicilioPaciente> DomiciliosPacientes { get; set; }
  public DbSet<Consultorio> Consultorios { get; set; }
  public DbSet<Cargos> Cargos { get; set; }
}
