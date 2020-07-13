using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public delegate Task<bool> RpcResponseHandler(RpcServerResponse response);
  public delegate void RpcRequestHandler(RpcServerRequest request, RpcResponseHandler response);


  public abstract class PeerRPCServer
  {
    protected Link Link { get; set; }
    public string Service { get; protected set; }
    public int Port { get; protected set; }

    protected event RpcRequestHandler RequestReceived;
    protected List<RpcRequestHandler> RequestHandler { get; set; }
    public Task ListenerTask { get; protected set; }

    protected Timer AnnounceInterval { get; set; }
    protected int AnnouncePeriod { get; set; }

    public PeerRPCServer(Link link, int announcePeriod = 120 * 1000)
    {
      Link = link;
      AnnouncePeriod = announcePeriod;
      RequestHandler = new List<RpcRequestHandler>();
    }

    public async Task<bool> Listen(string service, int port)
    {
      this.Service = service;
      this.Port = port;

      var started = await StartServer();
      if (!started) return false;

      AnnounceInterval = new Timer(async (_) =>
      {
        await Link.Announce(this.Service, this.Port);
      }, null, 0, AnnouncePeriod);

      return true;
    }

    public async Task Close()
    {
      if (AnnounceInterval != null) await AnnounceInterval.DisposeAsync();
      await StopServer();
      foreach (var handler in RequestHandler)
      {
        RequestReceived -= handler;
      }
      RequestHandler.Clear();
    }

    public void AddRequestHandler(RpcRequestHandler handler)
    {
      RequestReceived += handler;
      this.RequestHandler.Add(handler);
    }

    public void RemoveRequestHandler(RpcRequestHandler handler)
    {
      RequestReceived -= handler;
      this.RequestHandler.Remove(handler);
    }

    protected virtual void OnRequestReceived(RpcServerRequest request)
    {
      RequestReceived?.Invoke(request, this.SendResponse);
    }

    protected abstract Task<bool> StartServer();
    protected abstract Task StopServer();
    protected abstract Task<bool> SendResponse(RpcServerResponse response);
  }
}
