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
      var consultoriosList = await _context.Consultorios.ToListAsync();

      return new ResumenPersonalResponse { MedicosActivos = medicosActivos, TecnicosActivos = tecnicosActivos, Consultorios = consultorios, PersonalCaja = personalCaja, Cargos = cargos, ConsultoriosList = consultoriosList };
    }

    public async Task<IEnumerable<PersonalRegistradoResponse>> BuscarPersonalAsync(PersonalFiltroRequest filtro)
    {
      var resultado = new List<PersonalRegistradoResponse>();

      bool buscarTodos = filtro.TipoPersonal?.Equals("TODOS", StringComparison.OrdinalIgnoreCase) == true;

      // BUSCAR EN MÉDICOS
      if (buscarTodos || filtro.TipoPersonal?.Equals("Medico General", StringComparison.OrdinalIgnoreCase) == true)
      {
        var queryMedicos = _context.Medicos.AsQueryable();

        if (filtro.IdConsultorio.HasValue && filtro.IdConsultorio.Value != 0)
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
      if (buscarTodos || filtro.TipoPersonal?.Equals("Enfermeria", StringComparison.OrdinalIgnoreCase) == true)
      {
        var queryEnfermeria = _context.Enfermerias
            .Include(e => e.Personal)
            .AsQueryable();

        if (!buscarTodos)
        {
          queryEnfermeria = queryEnfermeria.Where(e => e.Personal.Cargo.ToUpper().Contains("ENFERMERIA"));
        }

        if (filtro.IdConsultorio.HasValue && filtro.IdConsultorio.Value != 0)
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
      // Solo buscar personal técnico (Administrador/Cajero) si no se filtró por consultorio
      bool buscarPersonalTecnico = (!filtro.IdConsultorio.HasValue || filtro.IdConsultorio.Value == 0);

      // BUSCAR EN PERSONAL TÉCNICO: ADMINISTRADOR
      if ((buscarTodos || filtro.TipoPersonal?.Equals("Administrador", StringComparison.OrdinalIgnoreCase) == true)
          && buscarPersonalTecnico) // <-- solo si no hay IdConsultorio
      {
        var tecnicos = _context.PersonalTecnicos.AsQueryable();

        if (!buscarTodos)
          tecnicos = tecnicos.Where(pt => pt.Cargo.ToUpper().Contains("ADMINISTRADOR"));

        var list = await tecnicos.Select(pt => new
        {
          Id = pt.IdPersonal,
          Dni = pt.NumeroDni,
          Nombre = pt.Nombre,
          ApellidoPaterno = pt.ApellidoPaterno,
          ApellidoMaterno = pt.ApellidoMaterno,
          Cargo = pt.Cargo,
          Estado = pt.EstadoLaboral,
          Telefono = pt.Telefono
        }).ToListAsync();

        resultado.AddRange(list.Select(t => new PersonalRegistradoResponse
        {
          Id = t.Id,
          Dni = t.Dni,
          NombresApellidos = $"{t.Nombre} {t.ApellidoPaterno} {t.ApellidoMaterno}",
          Cargo = t.Cargo,
          Estado = t.Estado,
          Telefono = t.Telefono
        }));
      }

      // BUSCAR EN PERSONAL TÉCNICO: CAJERO
      if ((buscarTodos || filtro.TipoPersonal?.Equals("Cajero", StringComparison.OrdinalIgnoreCase) == true)
          && buscarPersonalTecnico) // <-- solo si no hay IdConsultorio
      {
        var tecnicos = _context.PersonalTecnicos.AsQueryable();

        if (!buscarTodos)
          tecnicos = tecnicos.Where(pt => pt.Cargo.ToUpper().Contains("CAJERO"));

        var list = await tecnicos.Select(pt => new
        {
          Id = pt.IdPersonal,
          Dni = pt.NumeroDni,
          Nombre = pt.Nombre,
          ApellidoPaterno = pt.ApellidoPaterno,
          ApellidoMaterno = pt.ApellidoMaterno,
          Cargo = pt.Cargo,
          Estado = pt.EstadoLaboral,
          Telefono = pt.Telefono
        }).ToListAsync();

        resultado.AddRange(list.Select(t => new PersonalRegistradoResponse
        {
          Id = t.Id,
          Dni = t.Dni,
          NombresApellidos = $"{t.Nombre} {t.ApellidoPaterno} {t.ApellidoMaterno}",
          Cargo = t.Cargo,
          Estado = t.Estado,
          Telefono = t.Telefono
        }));
      }


      // FILTROS GENERALES
      var queryFinal = resultado.AsQueryable();

      if (!string.IsNullOrEmpty(filtro.Dni))
      {
        queryFinal = queryFinal.Where(p => p.Dni.Contains(filtro.Dni));
      }

      if (!string.IsNullOrEmpty(filtro.Estado))
      {
        queryFinal = queryFinal.Where(p => p.Estado.Equals(filtro.Estado, StringComparison.OrdinalIgnoreCase));
      }

      if (!string.IsNullOrEmpty(filtro.Nombre))
      {
        var nombreUpper = filtro.Nombre.ToUpper();
        queryFinal = queryFinal.Where(p => p.NombresApellidos.ToUpper().Contains(nombreUpper));
      }

      return queryFinal.OrderBy(p => p.NombresApellidos).ToList();
    }


  }
}
