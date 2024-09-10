using System;
using System.Threading.Tasks;
using System.Linq;
using Grenache.Interfaces;

namespace Grenache.Example.Client
{
  class Point2D
  {
    public int X { get; set; }
    public int Y { get; set; }
  }

  class RpcRequest
  {
    public string Action { get; set; }
    public object[] Args { get; set; }
  }

  class Program
  {
    static async Task Main(string[] args)
    {
      Utils.HttpUtil.SetClient(new System.Net.Http.HttpClient());

      Link link = new("http://127.0.0.1:30001");
      HttpPeerRPCClient client = new(link);

      Console.WriteLine("Request: rpc_ping hello");
      var rpcRes = await client.Request("rpc_ping",
        """
        {
            "action": "Greet",
            "args": ["hello"]
        }
        """);
      Console.WriteLine("Response: " + rpcRes.Data);

      Console.WriteLine("Request: rpc_ping async hello");
      rpcRes = await client.Request("rpc_ping",
        """
        {
            "action": "GreetAsync",
            "args": ["hello"]
        }
        """);
      Console.WriteLine("Response: " + rpcRes.Data);

      Console.WriteLine("Request: rpc_ping error");
      try
      {
        rpcRes = await client.Request("rpc_ping",
          """
          {
              "action": "GreetAsync123",
              "args": ["hello"]
          }
          """);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

      Console.WriteLine(
        "Request: rpc_ping Action = \"Point\", Args = [new {point = new { x = \"10\" }}] Multiplies by 5");
      rpcRes = await client.Request("rpc_ping", new RpcRequest
      {
        Action = "Point",
        Args = [new { x = 10 }]
      });
      Console.WriteLine("Response: " + rpcRes.Data);

      Console.WriteLine(
        "Request: rpc_ping Action = \"Point\", Args = [new {point = new { x = \"-5\" }}] throws an error as x should be positive");
      try
      {
        rpcRes = await client.Request("rpc_ping", new RpcRequest
        {
          Action = "Point",
          Args = [new { x = -5 }]
        });
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

      Console.WriteLine(
        "Request: rpc_ping Action = \"Point2D\", Args = [new {point = new Point2D { X = \"5\", Y = \"10\" }}] Multiplies by 3");
      rpcRes = await client.Request("rpc_ping", new RpcRequest
      {
        Action = "Point2D",
        Args = [new Point2D { X = 5, Y = 10 }]
      });
      Console.WriteLine("Response: " + rpcRes.Data);

      var point = IRPCClient.ParseRpcResponseData<Point2D>(rpcRes);
      Console.WriteLine($"Parsed object: x: {point.X}, y: {point.Y}");

      Console.WriteLine("Map Request: rpc_ping test map");
      var mapRpcRes = await client.Map("rpc_ping", new RpcRequest
      {
        Action = "Point2D",
        Args = [new Point2D { X = 5, Y = 10 }]
      });
      Console.WriteLine("Mapped Response: " + string.Join(",", mapRpcRes.Select(x => x.Data)));
    }
  }
}
