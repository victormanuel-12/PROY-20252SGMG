using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SGMG.common.exception;
using SGMG.Dtos.Response;

namespace SGMG.common.exception
{
  public class ValidationFilter : IActionFilter
  {
    public void OnActionExecuting(ActionExecutingContext context)
    {
      if (!context.ModelState.IsValid)
      {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new
            {
              Field = e.Key,
              Errors = e.Value?.Errors.Select(er => er.ErrorMessage).ToArray()
            });

        var response = new GenericResponse<object>
        {
          Success = false,
          Message = "Errores de validación en la solicitud.",
          Data = errors
        };

        context.Result = new BadRequestObjectResult(response);
      }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
      // No se necesita implementar
    }
  }
}