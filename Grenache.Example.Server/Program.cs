using System;
using System.Threading.Tasks;
using Grenache.Models.PeerRPC;
using Grenache.Utils;

namespace Grenache.Example.Server
{
  class Program
  {
    static HttpPeerRPCServer _server;

    static async Task Main(string[] args)
    {
      HttpUtil.SetClient(new System.Net.Http.HttpClient());

      Link link = new("http://127.0.0.1:30001");
      var pingService = new RpcPingService();
      var actionHandler = new RpcActionHandler(pingService.GetType().Assembly);
      _server = new HttpPeerRPCServer(link, 10000);
      _server.AddRequestHandler((req, res) =>
      {
        var resultDelegate = actionHandler.HandleAction(req.Payload);
        var data = resultDelegate(pingService);
        res.Invoke(new RpcServerResponse { RId = req.RId, Data = data });
      });
      var started = await _server.Listen("rpc_ping", 7070);
      if (!started) throw new Exception("ERR_SERVER_STARTUP_FAILURE");
      Console.WriteLine("Server started!");

      CloseHandler();

      await _server.ListenerTask; // used to keep the app always running
    }

    static void CloseHandler()
    {
      Task.Factory.StartNew(async () =>
      {
        Console.WriteLine("Press any key to close the server");
        Console.Read();
        await _server.Close();
      });
    }
  }
}
