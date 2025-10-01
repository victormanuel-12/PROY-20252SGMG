using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGMG.Dtos.Request;
using SGMG.Dtos.Response;
using SGMG.Services;

namespace SGMG.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class PersonalController : Controller
  {
    private readonly ILogger<PersonalController> _logger;
    private readonly IPersonalService _personalService;

    public PersonalController(ILogger<PersonalController> logger, IPersonalService personalService)
    {
      _logger = logger;
      _personalService = personalService;
    }

    [HttpGet]
    [Route("/personal/resumen")]
    public async Task<ActionResult<GenericResponse<ResumenPersonalResponse>>> GetResumenPersonal()
    {
      return await _personalService.GetResumenPersonalAsync();
    }

    [HttpPost]
    [Route("/personal/buscar")]
    public async Task<ActionResult<GenericResponse<IEnumerable<PersonalRegistradoResponse>>>> BuscarPersonal([FromBody] PersonalFiltroRequest filtro)
    {
      return await _personalService.BuscarPersonalAsync(filtro);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View("Error!");
    }
  }
}