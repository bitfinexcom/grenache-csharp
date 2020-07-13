using System;
using System.Threading.Tasks;
using Grenache;
using Grenache.Models.PeerRPC;

namespace Grenache.Example.Server
{
  class Program
  {
    static HttpPeerRPCServer server;
    static async Task Main(string[] args)
    {
      Link link = new Link("http://127.0.0.1:30001");
      server = new HttpPeerRPCServer(link, 10000);
      server.AddRequestHandler((req, res) =>
      {
        res.Invoke(new RpcServerResponse { RId = req.RId, Data = req.Payload });
      });
      var started = await server.Listen("rpc_ping", 7070);
      if (!started) throw new Exception("Couldn't start the server!");
      Console.WriteLine("Server started!");

      CloseHandler();

      await server.ListenerTask; // used to keep the app always running
    }

    static void CloseHandler()
    {
      Task.Factory.StartNew(async () =>
      {
        Console.WriteLine("Press any key to close the server");
        Console.Read();
        await server.Close();
      });
    }
  }
}
