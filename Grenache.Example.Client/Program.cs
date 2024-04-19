using System;
using System.Threading.Tasks;
using System.Linq;

namespace Grenache.Example.Client
{
  class Point2D
  {
    public int X { get; set; }
    public int Y { get; set; }
  }

  class Program
  {
    static async Task Main(string[] args)
    {
      Utils.HttpUtil.SetClient(new System.Net.Http.HttpClient());

      Link link = new("http://127.0.0.1:30001");
      HttpPeerRPCClient client = new(link);

      Console.WriteLine("Request: rpc_ping hello");
      var rpcRes = await client.Request("rpc_ping", "hello");
      Console.WriteLine("Response: " + rpcRes.Data);

      Console.WriteLine("Request: rpc_ping new { x = \"hello\" }");
      rpcRes = await client.Request("rpc_ping", new { x = "hello" });
      Console.WriteLine("Response: " + rpcRes.Data);

      Console.WriteLine("Request: rpc_ping new Point2D() { X = 5, Y = 10 }");
      rpcRes = await client.Request("rpc_ping", new Point2D() { X = 5, Y = 10 });
      Console.WriteLine("Response: " + rpcRes.Data);
      var point = PeerRPCClient.ParseRpcResponseData<Point2D>(rpcRes);
      Console.WriteLine($"Parsed object: x: {point.X}, y: {point.Y}");

      Console.WriteLine("Map Request: rpc_ping test map");
      var mapRpcRes = await client.Map("rpc_ping", "test map");
      Console.WriteLine("Mapped Response: " + string.Join(",", mapRpcRes.Select(x => x.Data)));
    }
  }
}
