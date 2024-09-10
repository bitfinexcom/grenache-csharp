using System;
using Grenache.Utils;
using Xunit;
using System.Text.Json;

namespace Grenache.Test;

public class RpcActionHandlerTests
{
  private readonly RpcActionHandler _handler;

  public RpcActionHandlerTests()
  {
    _handler = new RpcActionHandler(typeof(TestClass));
  }

  [Fact]
  public void HandleAction_ValidMethodAndParameters_ReturnsValidDelegate()
  {
    // Arrange
    const string json = """
                                {
                                    "action": "TestMethod",
                                    "args": [ "value1", 123 ]
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
    const string json = """
                        
                                {
                                    "action": "NonExistentMethod",
                                    "args": [{}]
                                }
                        """;

    // Assert
    Assert.Throws<InvalidOperationException>(() => _handler.HandleAction(json));
  }

  [Fact]
  public void HandleAction_MissingRequiredParameters_ThrowsArgumentException()
  {
    // Arrange
    const string json = """
                        
                                {
                                    "action": "TestMethod",
                                    "args": [ "value1" ]
                                }
                        """;

    // Assert
    var testClassInstance = new TestClass();

    // Assert not throws an exception
    Assert.ThrowsAsync<ArgumentException>(async () =>
    {
      var resultDelegate = _handler.HandleAction(json);
      await resultDelegate(testClassInstance);
    });
  }

  [Fact]
  public void HandleAction_InvalidJson_ThrowsJsonException()
  {
    // Arrange
    const string json = "Invalid JSON Format";

    // Assert
    Assert.ThrowsAny<JsonException>(() => _handler.HandleAction(json));
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
    if (message.Equals("hello", StringComparison.CurrentCultureIgnoreCase))
      return "world";
    return string.Empty;
  }
}
