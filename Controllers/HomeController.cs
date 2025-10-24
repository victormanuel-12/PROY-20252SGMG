using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGMG.Models;

namespace SGMG.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;

  public HomeController(ILogger<HomeController> logger)
  {
    _logger = logger;
  }

  public IActionResult Index()
  {
    return View();
  }
  public IActionResult Personal()
  {
    return View();
  }

  public IActionResult Receta()
  {
    _logger.LogInformation("Accediendo a la vista de generación de receta médica.");
    return View();
  }


  [HttpGet("Home/HistorialRecetas/{idPaciente}")]
  public IActionResult HistorialRecetas(int idPaciente)
  {
    if (idPaciente <= 0)
    {
      TempData["Error"] = "ID de paciente inválido";
      return RedirectToAction("Index");
    }

    // Envía el ID a la vista, útil si vas a hacer una consulta en JS o Razor
    ViewBag.IdPaciente = idPaciente;

    return View();
  }

  public IActionResult Paciente()
  {
    return View();
  }
  public IActionResult VisualCitas(int? idPaciente)
  {
    ViewBag.IdPaciente = idPaciente;
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }
  public IActionResult Historial()
  {
    return View();
  }
  public IActionResult HorarioMedico(int? idMedico, int? idPaciente, int? semana)
  {
    if (idMedico == null || idMedico == 0)
    {
      return RedirectToAction("VisualCitas");
    }
    ViewBag.Semana = semana ?? 0;
    ViewBag.IdMedico = idMedico;
    ViewBag.IdPaciente = idPaciente;
    ViewData["Title"] = $"Horario del Médico - ID: {idMedico}";

    return View();
  }


  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error(string? mensaje = null)
  {
    ViewBag.ErrorMessage = mensaje ?? "Ha ocurrido un error inesperado. Por favor, inténtalo nuevamente.";
    return View("ErrorCustom");
  }
  public IActionResult Error404()
  {
    Response.StatusCode = 404;
    return View();
  }


}
