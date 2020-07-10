using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public class HttpPeerRPCServer : PeerRPCServer
  {
    protected HttpListener Listener { get; set; }
    protected ConcurrentDictionary<string, HttpListenerResponse> RequestMap { get; set; }

    public HttpPeerRPCServer(Link link, int announcePeriod = 120 * 1000) : base(link, announcePeriod) { }

    protected override Task<bool> StartServer()
    {
      var url = $"http://+:{Port}/";
      Listener = new HttpListener();
      Listener.Prefixes.Add(url);
      RequestMap = new ConcurrentDictionary<string, HttpListenerResponse>();
      ListenerTask = MainTask();

      return Task.FromResult(true);
    }

    protected async Task MainTask()
    {
      Listener.Start();

      while (Listener.IsListening)
      {
        try
        {
          var context = await Listener.GetContextAsync();
          lock (Listener)
          {
            if (Listener.IsListening) ProcessRequest(context);
          }
        }
        catch
        {
        }
      }
    }

    protected void ProcessRequest(HttpListenerContext context)
    {
      var responseHandler = context.Response;
      try
      {
        if (context.Request.HttpMethod.ToUpper() != "POST") throw new Exception("Invalid HTTP Method");

        using (var body = context.Request.InputStream)
        using (var reader = new StreamReader(body, context.Request.ContentEncoding))
        {
          var json = reader.ReadToEnd();
          var req = RpcServerRequest.FromArray(JsonConvert.DeserializeObject<object[]>(json));
          RequestMap.TryAdd(req.RId.ToString(), responseHandler);
          OnRequestReceived(req);
        }
      }
      catch (Exception e)
      {
        responseHandler.StatusCode = 500;
        responseHandler.ContentType = "application/json";
        var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
        responseHandler.ContentLength64 = buffer.Length;
        responseHandler.OutputStream.Write(buffer, 0, buffer.Length);
        responseHandler.Close();
      }
    }

    protected override async Task StopServer()
    {
      if (Listener.IsListening) Listener.Close();
      await ListenerTask;
    }

    protected override async Task<bool> SendResponse(RpcServerResponse response)
    {
      var key = response.RId.ToString();
      if (!RequestMap.ContainsKey(key)) return false;

      HttpListenerResponse responseHandler;
      RequestMap.Remove(key, out responseHandler);

      var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response.ToArray()));

      responseHandler.StatusCode = 200;
      responseHandler.ContentType = "application/json";
      responseHandler.ContentLength64 = buffer.Length;
      await responseHandler.OutputStream.WriteAsync(buffer, 0, buffer.Length);

      responseHandler.Close();
      return true;
    }
  }
}
