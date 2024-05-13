using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Grenache.Utils;

public class RpcActionHandler(Assembly targetAssembly)
{
    public Func<object, object> HandleAction(string json)
    {
        var jsonObject = JObject.Parse(json);
        
        if (!jsonObject.TryGetValue("action", StringComparison.OrdinalIgnoreCase, out JToken actionToken))
        {
            throw new ArgumentException("JSON must include an 'action' property.");
        }
        var action = actionToken.ToString();

        if (!jsonObject.TryGetValue("args", StringComparison.OrdinalIgnoreCase, out JToken argsToken))
        {
            throw new ArgumentException("JSON must include an 'args' property.");
        }
        var args = (JArray)argsToken;

        var theMethod = FindMethodInAssembly(action, args);

        if (theMethod == null)
        {
            throw new InvalidOperationException($"No suitable method found for action: {action}");
        }

        var methodArgs = PrepareMethodArguments(theMethod, args);
        return instance => theMethod.Invoke(instance, methodArgs);
    }


    private MethodInfo? FindMethodInAssembly(string action, JArray args)
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
                    args.Children<JObject>().Any(arg =>
                        arg.Properties().Any(prop => prop.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
                );

                if (allRequiredParamsPresent)
                {
                    return method;
                }

                throw new ArgumentException($"Missing required parameter.");
            }
        }

        return null;
    }

    private object?[] PrepareMethodArguments(MethodInfo method, JArray args)
    {
        var parameters = method.GetParameters();
        var methodArgs = new object?[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var arg = args.Children<JObject>().FirstOrDefault(a =>
                a.Properties().Any(p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase)));

            if (arg != null)
            {
                if (param.Name != null)
                {
                    var value = arg[param.Name];
                    if (param.ParameterType == typeof(string))
                    {
                        methodArgs[i] = value.ToString();
                    }
                    else
                    {
                        try
                        {
                            methodArgs[i] = value.ToObject(param.ParameterType); 
                        }
                        catch (InvalidCastException e)
                        {
                            throw new ArgumentException($"Cannot convert parameter '{param.Name}' to type {param.ParameterType.Name}");
                        }
                    }
                }
            }
            else if (param.HasDefaultValue)
            {
                methodArgs[i] = param.DefaultValue;
            }
            else
            {
                throw new ArgumentException($"Missing required parameter: {param.Name}");
            }
        }

        return methodArgs;
    }
}