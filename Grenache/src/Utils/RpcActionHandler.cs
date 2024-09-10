using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grenache.Utils;

public class RpcActionHandler(Type type)
{
  public Func<object, Task<object>> HandleAction(string json)
  {
    var jsonDocument = JsonDocument.Parse(json);

    if (!jsonDocument.RootElement.TryGetProperty("action", out var actionElement))
    {
      throw new ArgumentException("ERR_JSON_ACTION_REQUIRED: 'action' property is required in the JSON object");
    }

    var action = actionElement.ToString();

    var argsElement = FindElement(jsonDocument.RootElement, "args");

    if (argsElement is null)
    {
      throw new ArgumentException("ERR_JSON_ARGS_REQUIRED: 'args' property is required in the JSON object");
    }

    var theMethod = FindMethodInType(action);

    if (theMethod == null)
    {
      throw new InvalidOperationException(
        $"ERR_METHOD_NOT_FOUND_FOR_ACTION: No suitable method found for action: {action}");
    }

    var methodArgs = PrepareMethodArguments(theMethod, argsElement.Value);

    var isAsync = typeof(Task).IsAssignableFrom(theMethod.ReturnType);

    return async instance =>
    {
      var result = theMethod.Invoke(instance, methodArgs);

      if (isAsync)
      {
        await (Task)result;

        // If the method returns Task<T>, get the result using reflection
        if (theMethod.ReturnType.IsGenericType && theMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
          var resultProperty = theMethod.ReturnType.GetProperty("Result");
          return resultProperty.GetValue(result);
        }

        return null; // The method is just Task, not Task<T>
      }
      else
      {
        return result;
      }
    };
  }

  private static JsonElement? FindElement(JsonElement element, string propertyName)
  {
    if (element.ValueKind != JsonValueKind.Object)
    {
      throw new ArgumentException("The provided JsonElement is not an array.", nameof(element));
    }

    foreach (var property in element.EnumerateObject())
    {
      if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
      {
        return property.Value;
      }
    }

    return null;
  }

  private static JsonElement? FindPropertyInArray(JsonElement element, int index)
  {
    // Check if the JsonElement is an array
    if (element.ValueKind != JsonValueKind.Array)
    {
      throw new ArgumentException("The provided JsonElement is not an array.", nameof(element));
    }

    if (element.GetArrayLength() <= index)
    {
      return null;
    }

    return element[index];
  }


  private MethodInfo? FindMethodInType(string action)
  {
    var method = type
      .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
      .SingleOrDefault(m => m.Name.Equals(action, StringComparison.OrdinalIgnoreCase));

    if (method != null)
    {
      return method;
    }

    throw new InvalidOperationException("ERR_INVALID_ACTION");
  }

  private object?[] PrepareMethodArguments(MethodInfo method, JsonElement args)
  {
    var parameters = method.GetParameters();
    var methodArgs = new object?[parameters.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
      var param = parameters[i];
      var prop = FindPropertyInArray(args, i);

      if (prop != null)
      {
        if (prop.Value.ValueKind != JsonValueKind.Undefined)
        {
          // Handle type conversion based on parameter type
          try
          {
            if (param.ParameterType == typeof(string))
            {
              methodArgs[i] = prop.Value.GetString();
            }
            else if (param.ParameterType == typeof(int))
            {
              methodArgs[i] = prop.Value.GetInt32();
            }
            else
            {
              if (param.ParameterType == typeof(bool))
              {
                methodArgs[i] = prop.Value.GetBoolean();
              }
              else
              {
                var options = new JsonSerializerOptions
                {
                  PropertyNameCaseInsensitive = true
                };
                methodArgs[i] = JsonSerializer.Deserialize(prop.Value.GetRawText(), param.ParameterType, options);
              }
            }
          }
          catch (Exception e) when (e is JsonException || e is InvalidCastException)
          {
            throw new ArgumentException(
              $"ERR_TYPE_CONVERSION_FAILED: Cannot convert parameter '{param.Name}' to type {param.ParameterType.Name}");
          }
        }
        else if (param.HasDefaultValue)
        {
          methodArgs[i] = param.DefaultValue;
        }
        else
        {
          throw new ArgumentException($"ERR_MISSING_PARAMETER: Missing required parameter: {param.Name}");
        }
      }
    }

    return methodArgs;
  }
}
