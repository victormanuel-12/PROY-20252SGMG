using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SGMG.Dtos.Response;

namespace SGMG.common.middleware
{
  /// <summary>
  /// Middleware para pre-validar JSON contra el tipo de DTO esperado y capturar múltiples errores
  /// </summary>
  public class JsonValidationMiddleware
  {
    private readonly RequestDelegate _next;

    public JsonValidationMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      // Solo procesar POST, PUT, PATCH con JSON
      if (context.Request.ContentType?.Contains("application/json") == true &&
          (context.Request.Method == "POST" ||
           context.Request.Method == "PUT" ||
           context.Request.Method == "PATCH"))
      {
        // Obtener el tipo esperado del parámetro del action
        var expectedType = GetExpectedBodyType(context);

        if (expectedType != null)
        {
          context.Request.EnableBuffering();

          using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

          var body = await reader.ReadToEndAsync();
          context.Request.Body.Position = 0;

          // Validar el JSON contra el tipo esperado
          var errors = ValidateJsonAgainstType(body, expectedType);

          if (errors.Any())
          {
            var response = new GenericResponse<object>
            {
              Success = false,
              Message = "Errores de tipo de dato en la solicitud.",
              Data = errors
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
            return;
          }
        }
      }

      await _next(context);
    }

    /// <summary>
    /// Obtiene el tipo esperado del body del endpoint actual
    /// </summary>
    private Type GetExpectedBodyType(HttpContext context)
    {
      var endpoint = context.GetEndpoint();
      if (endpoint == null) return null;

      var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
      if (actionDescriptor == null) return null;

      // Buscar el parámetro que viene del Body
      var bodyParameter = actionDescriptor.Parameters
        .FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body);

      return bodyParameter?.ParameterType;
    }

    /// <summary>
    /// Valida el JSON contra el tipo esperado y retorna todos los errores encontrados
    /// </summary>
    private List<object> ValidateJsonAgainstType(string jsonBody, Type expectedType)
    {
      var errors = new Dictionary<string, List<string>>();

      try
      {
        using var document = JsonDocument.Parse(jsonBody);
        var root = document.RootElement;

        // Obtener todas las propiedades del tipo esperado
        var properties = expectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Validar cada propiedad del JSON contra el tipo esperado
        foreach (var property in root.EnumerateObject())
        {
          var propertyInfo = properties.FirstOrDefault(p =>
            string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));

          if (propertyInfo != null)
          {
            ValidateProperty(property.Value, property.Name, propertyInfo.PropertyType, errors);
          }
        }
      }
      catch (JsonException)
      {
        errors["request"] = new List<string> { "El formato JSON es inválido." };
      }

      // Convertir el diccionario a la estructura de respuesta
      return errors.Select(e => new
      {
        Field = e.Key,
        Errors = e.Value.ToArray()
      }).Cast<object>().ToList();
    }

    /// <summary>
    /// Valida una propiedad contra su tipo esperado
    /// </summary>
    private void ValidateProperty(JsonElement element, string propertyName, Type expectedType, Dictionary<string, List<string>> errors)
    {
      // Obtener el tipo subyacente si es Nullable
      var underlyingType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;

      // Si es null y el tipo no es nullable, es un error (manejado por DataAnnotations)
      if (element.ValueKind == JsonValueKind.Null)
      {
        return; // DataAnnotations maneja required
      }

      // Validar según el tipo esperado
      if (underlyingType == typeof(int) || underlyingType == typeof(long))
      {
        if (element.ValueKind != JsonValueKind.Number || !TryParseInteger(element))
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un número entero válido.");
        }
      }
      else if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float))
      {
        if (element.ValueKind != JsonValueKind.Number)
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un número decimal válido.");
        }
      }
      else if (underlyingType == typeof(bool))
      {
        if (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False)
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un valor booleano (true/false).");
        }
      }
      else if (underlyingType == typeof(DateTime))
      {
        if (element.ValueKind != JsonValueKind.String || !element.TryGetDateTime(out _))
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser una fecha válida (formato: YYYY-MM-DD o YYYY-MM-DDTHH:mm:ss).");
        }
      }
      else if (underlyingType == typeof(Guid))
      {
        if (element.ValueKind != JsonValueKind.String || !element.TryGetGuid(out _))
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un identificador único (GUID) válido.");
        }
      }
      else if (underlyingType == typeof(string))
      {
        if (element.ValueKind != JsonValueKind.String)
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser una cadena de texto válida.");
        }
      }
      else if (underlyingType.IsEnum)
      {
        if (element.ValueKind == JsonValueKind.String)
        {
          var stringValue = element.GetString();
          if (!Enum.IsDefined(underlyingType, stringValue) && !int.TryParse(stringValue, out _))
          {
            var validValues = string.Join(", ", Enum.GetNames(underlyingType));
            AddError(errors, propertyName, $"El campo '{propertyName}' debe ser uno de los siguientes valores: {validValues}.");
          }
        }
        else if (element.ValueKind == JsonValueKind.Number)
        {
          if (!element.TryGetInt32(out var intValue) || !Enum.IsDefined(underlyingType, intValue))
          {
            var validValues = string.Join(", ", Enum.GetNames(underlyingType));
            AddError(errors, propertyName, $"El campo '{propertyName}' debe ser uno de los siguientes valores: {validValues}.");
          }
        }
        else
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un valor de enumeración válido.");
        }
      }
      else if (underlyingType.IsClass && underlyingType != typeof(string))
      {
        // Objeto anidado
        if (element.ValueKind == JsonValueKind.Object)
        {
          var nestedProperties = underlyingType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
          foreach (var nestedProp in element.EnumerateObject())
          {
            var nestedPropertyInfo = nestedProperties.FirstOrDefault(p =>
              string.Equals(p.Name, nestedProp.Name, StringComparison.OrdinalIgnoreCase));

            if (nestedPropertyInfo != null)
            {
              ValidateProperty(nestedProp.Value, $"{propertyName}.{nestedProp.Name}", nestedPropertyInfo.PropertyType, errors);
            }
          }
        }
        else
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser un objeto válido.");
        }
      }
      else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(List<>))
      {
        // Lista
        if (element.ValueKind != JsonValueKind.Array)
        {
          AddError(errors, propertyName, $"El campo '{propertyName}' debe ser una lista válida.");
        }
        else
        {
          var itemType = underlyingType.GetGenericArguments()[0];
          int index = 0;
          foreach (var item in element.EnumerateArray())
          {
            ValidateProperty(item, $"{propertyName}[{index}]", itemType, errors);
            index++;
          }
        }
      }
    }

    /// <summary>
    /// Intenta parsear un entero desde un JsonElement
    /// </summary>
    private bool TryParseInteger(JsonElement element)
    {
      return element.TryGetInt32(out _) || element.TryGetInt64(out _);
    }

    /// <summary>
    /// Agrega un error a la lista de errores
    /// </summary>
    private void AddError(Dictionary<string, List<string>> errors, string fieldName, string errorMessage)
    {
      // Limpiar el nombre del campo (quitar prefijos de objetos anidados si es necesario)
      var cleanFieldName = fieldName.Split('.').Last();

      if (!errors.ContainsKey(cleanFieldName))
      {
        errors[cleanFieldName] = new List<string>();
      }

      if (!errors[cleanFieldName].Contains(errorMessage))
      {
        errors[cleanFieldName].Add(errorMessage);
      }
    }
  }
}