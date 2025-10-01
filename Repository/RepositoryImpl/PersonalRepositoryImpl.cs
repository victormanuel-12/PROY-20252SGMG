using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGMG.Data;
using SGMG.Dtos.Request;
using SGMG.Dtos.Response;


namespace SGMG.Repository.RepositoryImpl
{
  public class PersonalRepositoryImpl : IPersonalRepository
  {
    private readonly ApplicationDbContext _context;

    public PersonalRepositoryImpl(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<ResumenPersonalResponse> GetResumenPersonalAsync()
    {
      var medicosActivos = await _context.Medicos
          .Where(m => m.EstadoLaboral == "Activo")
          .CountAsync();
      var tecnicosActivos = await _context.PersonalTecnicos
          .Where(pt => pt.EstadoLaboral == "Activo")
          .CountAsync();
      var personalCaja = await _context.PersonalTecnicos
     .Where(pt => pt.EstadoLaboral == "Activo" &&
                 (pt.Cargo.ToUpper().Contains("CAJA") ||
                  pt.Cargo.ToUpper().Contains("CAJERO")))
     .CountAsync();
      var consultorios = await _context.Consultorios.CountAsync();
      var cargos = await _context.Cargos.ToListAsync();
      return new ResumenPersonalResponse { MedicosActivos = medicosActivos, TecnicosActivos = tecnicosActivos, Consultorios = consultorios, PersonalCaja = personalCaja, Cargos = cargos };
    }

    public async Task<IEnumerable<PersonalRegistradoResponse>> BuscarPersonalAsync(PersonalFiltroRequest filtro)
    {
      var resultado = new List<PersonalRegistradoResponse>();

      // Determinar qué tablas consultar según TipoPersonal
      bool buscarMedicos = string.IsNullOrEmpty(filtro.TipoPersonal) ||
                          filtro.TipoPersonal.Equals("Medico General", StringComparison.OrdinalIgnoreCase);

      bool buscarEnfermeria = string.IsNullOrEmpty(filtro.TipoPersonal) ||
                             filtro.TipoPersonal.Equals("Enfermeria", StringComparison.OrdinalIgnoreCase);

      bool buscarTecnicos = string.IsNullOrEmpty(filtro.TipoPersonal) ||
                           filtro.TipoPersonal.Equals("Administrador", StringComparison.OrdinalIgnoreCase);

      bool buscarCajeros = string.IsNullOrEmpty(filtro.TipoPersonal) ||
                          filtro.TipoPersonal.Equals("Cajero", StringComparison.OrdinalIgnoreCase);

      // BUSCAR EN MÉDICOS
      if (buscarMedicos)
      {
        var queryMedicos = _context.Medicos
            .AsQueryable();

        // Filtrar por consultorio si aplica
        if (filtro.IdConsultorio.HasValue)
        {
          queryMedicos = queryMedicos.Where(m => m.IdConsultorio == filtro.IdConsultorio.Value);
        }

        var medicos = await queryMedicos
            .Select(m => new
            {
              Id = m.IdMedico,
              Dni = m.NumeroDni,
              Nombre = m.Nombre,
              ApellidoPaterno = m.ApellidoPaterno,
              ApellidoMaterno = m.ApellidoMaterno,
              Cargo = m.CargoMedico,
              Estado = m.EstadoLaboral,
              Telefono = m.Telefono
            })
            .ToListAsync();

        resultado.AddRange(medicos.Select(m => new PersonalRegistradoResponse
        {
          Id = m.Id,
          Dni = m.Dni,
          NombresApellidos = $"{m.Nombre} {m.ApellidoPaterno} {m.ApellidoMaterno}",
          Cargo = m.Cargo,
          Estado = m.Estado,
          Telefono = m.Telefono
        }));
      }

      // BUSCAR EN ENFERMERÍA
      if (buscarEnfermeria)
      {
        var queryEnfermeria = _context.Enfermerias
            .Include(e => e.Personal)
            .AsQueryable();

        // Filtrar por consultorio si aplica
        if (filtro.IdConsultorio.HasValue)
        {
          queryEnfermeria = queryEnfermeria.Where(e => e.IdConsultorio == filtro.IdConsultorio.Value);
        }

        var enfermeras = await queryEnfermeria
            .Select(e => new
            {
              Id = e.IdEnfermeria,
              Dni = e.Personal.NumeroDni,
              Nombre = e.Personal.Nombre,
              ApellidoPaterno = e.Personal.ApellidoPaterno,
              ApellidoMaterno = e.Personal.ApellidoMaterno,
              Cargo = e.Personal.Cargo,
              Estado = e.Personal.EstadoLaboral,
              Telefono = e.Personal.Telefono
            })
            .ToListAsync();

        resultado.AddRange(enfermeras.Select(e => new PersonalRegistradoResponse
        {
          Id = e.Id,
          Dni = e.Dni,
          NombresApellidos = $"{e.Nombre} {e.ApellidoPaterno} {e.ApellidoMaterno}",
          Cargo = e.Cargo,
          Estado = e.Estado,
          Telefono = e.Telefono
        }));
      }

      // BUSCAR EN PERSONAL TÉCNICO
      if (buscarTecnicos || buscarCajeros)
      {
        var queryTecnicos = _context.PersonalTecnicos
            .AsQueryable();

        // Si busca cajeros, filtrar solo por cargo
        if (buscarCajeros && !buscarTecnicos)
        {
          queryTecnicos = queryTecnicos.Where(pt =>
              pt.Cargo.ToUpper().Contains("ADMINISTRADOR") ||
              pt.Cargo.ToUpper().Contains("CAJERO"));
        }
        var tecnicos = await queryTecnicos
            .Select(pt => new
            {
              Id = pt.IdPersonal,
              Dni = pt.NumeroDni,
              Nombre = pt.Nombre,
              ApellidoPaterno = pt.ApellidoPaterno,
              ApellidoMaterno = pt.ApellidoMaterno,
              Cargo = pt.Cargo,
              Estado = pt.EstadoLaboral,
              Telefono = pt.Telefono
            })
            .ToListAsync();

        resultado.AddRange(tecnicos.Select(t => new PersonalRegistradoResponse
        {
          Id = t.Id,
          Dni = t.Dni,
          NombresApellidos = $"{t.Nombre} {t.ApellidoPaterno} {t.ApellidoMaterno}",
          Cargo = t.Cargo,
          Estado = t.Estado,
          Telefono = t.Telefono
        }));
      }

      // APLICAR FILTROS GENERALES
      // Si filtro.Dni tiene valor, busca coincidencias parciales
      // Aplicar filtros generales
      var query = resultado.AsQueryable();

      if (!string.IsNullOrEmpty(filtro.Dni))
      {
        query = query.Where(p => p.Dni.Contains(filtro.Dni));
      }

      if (!string.IsNullOrEmpty(filtro.Estado))
      {
        query = query.Where(p =>
            p.Estado.Equals(filtro.Estado, StringComparison.OrdinalIgnoreCase));
      }

      if (!string.IsNullOrEmpty(filtro.Nombre))
      {
        var nombreUpper = filtro.Nombre.ToUpper();
        query = query.Where(p =>
            p.NombresApellidos.ToUpper().Contains(nombreUpper));
      }

      resultado = query.ToList();

      return resultado.OrderBy(p => p.NombresApellidos);
    }
  }
}
