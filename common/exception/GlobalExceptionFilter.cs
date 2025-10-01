using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using SGMG.Dtos.Response;
using SGMG.common.exception;


namespace SGMG.common.exception
{
  public class GlobalExceptionFilter : IExceptionFilter
  {
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
      _logger = logger;
    }
    public void OnException(ExceptionContext context)
    {
      var exception = context.Exception;

      // Log del error
      _logger.LogError(exception, "Response Error: {Path}", context.HttpContext.Request.Path);

      // Imprimir stack trace (equivalente a printStackTrace())
      Console.WriteLine(exception.ToString());

      GenericResponse<object> response;
      int statusCode;

      // Manejo específico según el tipo de excepción
      if (exception is SqlException sqlException)
      {
        response = HandleSqlException(sqlException);
        statusCode = StatusCodes.Status409Conflict;
      }
      else
      {
        // Manejo genérico para todas las demás excepciones
        response = HandleAllException(exception);
        statusCode = StatusCodes.Status500InternalServerError;
      }

      context.Result = new ObjectResult(response)
      {
        StatusCode = statusCode
      };

      context.ExceptionHandled = true;
    }

    /// <summary>
    /// Equivalente a @ExceptionHandler(Exception.class)
    /// </summary>
    private GenericResponse<object> HandleAllException(Exception ex)
    {
      return new GenericResponse<object>
      {
        Success = false,
        Message = ex.Message,
        Data = null
      };
    }

    /// <summary>
    /// Equivalente a @ExceptionHandler(SQLException.class)
    /// </summary>
    private GenericResponse<object> HandleSqlException(SqlException ex)
    {
      _logger.LogError(ex, "SQL Error: {Message}", ex.Message);

      return new GenericResponse<object>
      {
        Success = false,
        Message = ex.Message,
        Data = null
      };
    }
  }
}
