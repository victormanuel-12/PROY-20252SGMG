using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SGMG.Dtos.Response;

namespace SGMG.common.exception
{
  public class ValidationFilter : IActionFilter
  {
    public void OnActionExecuting(ActionExecutingContext context)
    {

      //Si la acción o el controlador tiene el atributo [NoValidation], no ejecutar este filtro
        var cad = context.ActionDescriptor as ControllerActionDescriptor;
      if (cad != null)
      {
        var hasNoValidationOnMethod = cad.MethodInfo.GetCustomAttributes(typeof(NoValidationAttribute), true).Any();
        var hasNoValidationOnController = cad.ControllerTypeInfo.GetCustomAttributes(typeof(NoValidationAttribute), true).Any();
        if (hasNoValidationOnMethod || hasNoValidationOnController)
        {
          return;
        }
      
      // No validar en peticiones de solo lectura (GET, HEAD) para evitar errores de binding en query params
        var method = context.HttpContext.Request.Method;
      if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) || string.Equals(method, "HEAD", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      if (!context.ModelState.IsValid)
      {
        var errors = new List<object>();

        foreach (var entry in context.ModelState)
        {
          // Excluir errores del objeto raíz completo
          if (IsRootObjectError(entry.Key))
            continue;

          if (entry.Value?.Errors.Count > 0)
          {
            var fieldErrors = new List<string>();

            foreach (var error in entry.Value.Errors)
            {
              var friendlyMessage = GetFriendlyErrorMessage(error, entry.Key);
              if (!string.IsNullOrEmpty(friendlyMessage))
              {
                fieldErrors.Add(friendlyMessage);
              }
            }

            if (fieldErrors.Count > 0)
            {
              errors.Add(new
              {
                Field = CleanFieldName(entry.Key),
                Errors = fieldErrors.ToArray()
              });
            }
          }
        }

        var response = new GenericResponse<object>
        {
          Success = false,
          Message = "Errores de validación en la solicitud.",
          Data = errors
        };

          context.Result = new BadRequestObjectResult(response);
        }
      }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
      // No se necesita implementar
    }

    /// <summary>
    /// Verifica si es un error del objeto raíz (como "enfermeriaRequestDTO")
    /// </summary>
    private bool IsRootObjectError(string fieldName)
    {
      // Excluir solo si es exactamente el nombre del parámetro sin propiedades anidadas
      // y no empieza con $ (que indica propiedades JSON)
      return !fieldName.Contains(".") && !fieldName.StartsWith("$") &&
             (fieldName.EndsWith("DTO") || fieldName.EndsWith("Request") ||
              fieldName.EndsWith("RequestDTO") || char.IsLower(fieldName[0]));
    }

    /// <summary>
    /// Limpia el nombre del campo para que sea más legible
    /// </summary>
    private string CleanFieldName(string fieldName)
    {
      // Remover el prefijo "$." si existe
      if (fieldName.StartsWith("$."))
      {
        fieldName = fieldName.Substring(2);
      }

      // Si tiene el prefijo del DTO, removerlo (ejemplo: "enfermeriaRequestDTO.nombre" -> "nombre")
      if (fieldName.Contains("."))
      {
        var parts = fieldName.Split('.');
        // Tomar el último segmento (el nombre real del campo)
        fieldName = parts[parts.Length - 1];
      }

      return fieldName;
    }

    /// <summary>
    /// Obtiene un mensaje de error amigable
    /// </summary>
    private string GetFriendlyErrorMessage(ModelError error, string fieldName)
    {
      var cleanFieldName = CleanFieldName(fieldName);

      // Manejar errores de DataAnnotations personalizados primero
      if (!string.IsNullOrEmpty(error.ErrorMessage))
      {
        // Si el mensaje NO contiene indicadores técnicos, es un mensaje personalizado
        if (!error.ErrorMessage.Contains("could not be converted") &&
            !error.ErrorMessage.Contains("JSON value") &&
            !error.ErrorMessage.Contains("Path:") &&
            !error.ErrorMessage.Contains("LineNumber:") &&
            !error.ErrorMessage.Contains("BytePositionInLine:"))
        {
          // Si es el mensaje genérico de "required", personalizarlo
          if (error.ErrorMessage.Contains("field is required") ||
              error.ErrorMessage.Contains("is required"))
          {
            return $"El campo '{cleanFieldName}' es obligatorio.";
          }

          // Es un mensaje personalizado de DataAnnotations, retornarlo
          return error.ErrorMessage;
        }

        // Es un error técnico, procesarlo
        if (error.ErrorMessage.Contains("could not be converted") ||
            error.ErrorMessage.Contains("JSON value"))
        {
          return ParseJsonConversionError(error.ErrorMessage, cleanFieldName);
        }
      }

      // Si hay una excepción (errores de binding/conversión)
      if (error.Exception != null)
      {
        return GetTypeConversionError(error.Exception, cleanFieldName);
      }

      // Fallback genérico
      return $"El campo '{cleanFieldName}' no es válido.";
    }

    /// <summary>
    /// Obtiene el mensaje de error para conversiones de tipo desde excepciones
    /// </summary>
    private string GetTypeConversionError(Exception exception, string fieldName)
    {
      var exceptionMessage = exception.Message.ToLower();

      if (exceptionMessage.Contains("boolean"))
      {
        return $"El campo '{fieldName}' debe ser un valor booleano (true/false).";
      }

      if (exceptionMessage.Contains("int32") || exceptionMessage.Contains("int64"))
      {
        return $"El campo '{fieldName}' debe ser un número entero válido.";
      }

      if (exceptionMessage.Contains("decimal") || exceptionMessage.Contains("double") ||
          exceptionMessage.Contains("single"))
      {
        return $"El campo '{fieldName}' debe ser un número decimal válido.";
      }

      if (exceptionMessage.Contains("datetime"))
      {
        return $"El campo '{fieldName}' debe ser una fecha válida (formato: YYYY-MM-DD).";
      }

      if (exceptionMessage.Contains("guid"))
      {
        return $"El campo '{fieldName}' debe ser un identificador único (GUID) válido.";
      }

      return $"El campo '{fieldName}' tiene un formato incorrecto.";
    }

    /// <summary>
    /// Analiza errores de conversión JSON para obtener mensajes amigables
    /// </summary>
    private string ParseJsonConversionError(string errorMessage, string fieldName)
    {
      var lowerMessage = errorMessage.ToLower();

      if (lowerMessage.Contains("system.int32") || lowerMessage.Contains("nullable`1[system.int32]"))
      {
        return $"El campo '{fieldName}' debe ser un número entero válido.";
      }

      if (lowerMessage.Contains("system.int64") || lowerMessage.Contains("nullable`1[system.int64]"))
      {
        return $"El campo '{fieldName}' debe ser un número entero válido.";
      }

      if (lowerMessage.Contains("system.decimal") || lowerMessage.Contains("nullable`1[system.decimal]"))
      {
        return $"El campo '{fieldName}' debe ser un número decimal válido.";
      }

      if (lowerMessage.Contains("system.double") || lowerMessage.Contains("nullable`1[system.double]"))
      {
        return $"El campo '{fieldName}' debe ser un número decimal válido.";
      }

      if (lowerMessage.Contains("system.boolean") || lowerMessage.Contains("nullable`1[system.boolean]"))
      {
        return $"El campo '{fieldName}' debe ser un valor booleano (true/false).";
      }

      if (lowerMessage.Contains("system.datetime") || lowerMessage.Contains("nullable`1[system.datetime]"))
      {
        return $"El campo '{fieldName}' debe ser una fecha válida (formato: YYYY-MM-DD o YYYY-MM-DDTHH:mm:ss).";
      }

      if (lowerMessage.Contains("system.guid") || lowerMessage.Contains("nullable`1[system.guid]"))
      {
        return $"El campo '{fieldName}' debe ser un identificador único (GUID) válido.";
      }

      if (lowerMessage.Contains("system.string"))
      {
        return $"El campo '{fieldName}' debe ser una cadena de texto válida.";
      }

      return $"El campo '{fieldName}' tiene un formato de dato incorrecto.";
    }
  }
}