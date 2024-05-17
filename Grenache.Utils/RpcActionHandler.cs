using System.Reflection;
using System.Text.Json;

namespace Grenache.Utils;

public class RpcActionHandler(Assembly targetAssembly)
{
  public Func<object, object> HandleAction(string json)
  {
    var jsonDocument = JsonDocument.Parse(json);

    if (!jsonDocument.RootElement.TryGetProperty("action", out JsonElement actionElement))
    {
      throw new ArgumentException("ERR_JSON_ACTION_REQUIRED: 'action' property is required in the JSON object");
    }

    var action = actionElement.ToString();

    var argsElement = FindElement(jsonDocument.RootElement, "args");

    if (argsElement is null)
    {
      throw new ArgumentException("ERR_JSON_ARGS_REQUIRED: 'args' property is required in the JSON object");
    }

    var theMethod = FindMethodInAssembly(action, argsElement.Value);

    if (theMethod == null)
    {
      throw new InvalidOperationException(
        $"ERR_METHOD_NOT_FOUND_FOR_ACTION: No suitable method found for action: {action}");
    }

    var methodArgs = PrepareMethodArguments(theMethod, argsElement.Value);
    return instance => theMethod.Invoke(instance, methodArgs);
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
  
  private static JsonElement? FindPropertyInArray(JsonElement element, string propertyName)
  {
    // Check if the JsonElement is an array
    if (element.ValueKind != JsonValueKind.Array)
    {
      throw new ArgumentException("The provided JsonElement is not an array.", nameof(element));
    }

    // Iterate through each element in the array
    foreach (JsonElement item in element.EnumerateArray())
    {
      // Check if the current item is an object and search for the property
      if (item.ValueKind == JsonValueKind.Object)
      {
        foreach (var property in item.EnumerateObject())
        {
          if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
          {
            return property.Value;
          }
        }
      }
    }

    // Return null if no matching property is found in any object within the array
    return null;
  }


  private MethodInfo? FindMethodInAssembly(string action, JsonElement args)
  {
    foreach (var type in targetAssembly.GetTypes())
    {
      IEnumerable<MethodInfo?> methods = type
        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        .Where(m => m.Name.Equals(action, StringComparison.OrdinalIgnoreCase));

      foreach (var method in methods)
      {
        var parameters = method?.GetParameters();
        var allRequiredParamsPresent = parameters != null && parameters.All(p =>
          p.HasDefaultValue ||
          args.EnumerateArray().Any(arg =>
            arg.EnumerateObject().Any(prop => prop.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
        );

        if (allRequiredParamsPresent)
        {
          return method;
        }

        throw new ArgumentException("ERR_MISSING_PARAMETER: Missing required parameter.");
      }
    }

    return null;
  }

  private object?[] PrepareMethodArguments(MethodInfo method, JsonElement args)
  {
    var parameters = method.GetParameters();
    var methodArgs = new object?[parameters.Length];

    for (var i = 0; i < parameters.Length; i++)
    {
      var param = parameters[i];
      var prop = FindPropertyInArray(args, param.Name);

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
