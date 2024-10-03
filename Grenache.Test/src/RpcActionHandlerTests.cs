using System;
using Grenache.Utils;
using Xunit;
using System.Text.Json;
using System.Threading.Tasks;

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
  
  [Fact]
  public async Task HandleAction_UsesDefaultParameterValues_WhenNoArgsProvided()
  {
    // Arrange
    const string json = """
                                {
                                    "action": "TestMethodWithOptionalParam",
                                    "args": [ "value1" ]
                                }
                        """;

    // Act
    var resultDelegate = _handler.HandleAction(json);
    var testClassInstance = new TestClass();
    var result = await resultDelegate(testClassInstance);

    // Assert 
    Assert.Equal(100, result);
  }

  [Fact]
  public async Task HandleAction_DoesNotUseDefaultParameterValues_WhenArgsProvided()
  {
    // Arrange
    const string json = """
                                {
                                    "action": "TestMethodWithOptionalParam",
                                    "args": [ "value1", 7 ]
                                }
                        """;

    // Act
    var resultDelegate = _handler.HandleAction(json);
    var testClassInstance = new TestClass();
    var result = await resultDelegate(testClassInstance);

    // Assert 
    Assert.Equal(7, result);
  }

}

public class TestClass
{
  public void TestMethod(string param1, int param2)
  {
    Console.WriteLine($"TestMethod called with param1={param1} and param2={param2}");
  }

  public int TestMethodWithOptionalParam(string param1, int param2 = 100)
  {
    Console.WriteLine($"TestMethod called with param1={param1} and param2={param2}");
    return param2;
  }

  public string Greet(string message)
  {
    if (message.Equals("hello", StringComparison.CurrentCultureIgnoreCase))
      return "world";
    return string.Empty;
  }
}
