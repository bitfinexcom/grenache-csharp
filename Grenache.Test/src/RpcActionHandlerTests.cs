using System;
using System.Reflection;
using Grenache.Utils;
using Xunit;

namespace Grenache.Test;

public class RpcActionHandlerTests
{
    private readonly RpcActionHandler _handler;

    public RpcActionHandlerTests()
    {
        var testAssembly = Assembly.GetExecutingAssembly();
        _handler = new RpcActionHandler(testAssembly);
    }
[Fact]
    public void HandleAction_ValidMethodAndParameters_ReturnsValidDelegate()
    {
        // Arrange
        const string json = """
                                    {
                                        "action": "TestMethod",
                                        "args": [
                                            {
                                                "param1": "value1",
                                                "param2": 123
                                            }
                                        ]
                                    }
                            """;

        // Act
        var resultDelegate = _handler.HandleAction(json);
        var testClassInstance = new TestClass();

        // Assert not throws an exception
        resultDelegate(testClassInstance);
    }

    [Fact]
    public void HandleAction_MethodNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        const string json = @"
        {
            ""action"": ""NonExistentMethod"",
            ""args"": [{}]
        }";

        // Assert
        Assert.Throws<InvalidOperationException>(() => _handler.HandleAction(json));
    }

    [Fact]
    public void HandleAction_MissingRequiredParameters_ThrowsArgumentException()
    {
        // Arrange
        const string json = @"
        {
            ""action"": ""TestMethod"",
            ""args"": [
                {
                    ""param1"": ""value1""
                }
            ]
        }";

        // Assert
        Assert.Throws<ArgumentException>(() => _handler.HandleAction(json));
    }

    [Fact]
    public void HandleAction_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        const string json = "Invalid JSON Format";

        // Assert
        Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => _handler.HandleAction(json));
    }
}

public class TestClass
{
    public void TestMethod(string param1, int param2)
    {
        Console.WriteLine($"TestMethod called with param1={param1} and param2={param2}");
    }
    
    public string Greet(string message)
    {
        if (message.Equals("hello", System.StringComparison.CurrentCultureIgnoreCase))
            return "world";
        return string.Empty;
    }
}