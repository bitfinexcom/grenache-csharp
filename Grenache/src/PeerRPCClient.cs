using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public abstract class PeerRPCClient
  {
    protected Link Link { get; set; }
    protected long CacheAge { get; set; }
    protected Dictionary<string, LookupValue> Lookups { get; set; }

    public PeerRPCClient(Link link, long cacheAge = 120 * 1000)
    {
      Link = link;
      CacheAge = cacheAge;
      Lookups = new Dictionary<string, LookupValue>();
    }

    public async Task<RpcClientResponse> Request(string service, object payload)
    {
      var lookup = await ResolveService(service);
      if (lookup.Endpoints.Length == 0) throw new Exception("ERR_GRAPE_LOOKUP_EMPTY");

      var res = await CallRequest(service, lookup.Endpoints[0], payload);
      return res;
    }

    public async Task<RpcClientResponse[]> Map(string service, object payload)
    {
      var lookup = await ResolveService(service);
      if (lookup.Endpoints.Length == 0) throw new Exception("ERR_GRAPE_LOOKUP_EMPTY");

      var tasks = lookup.Endpoints.Select(ep => CallRequest(service, ep, payload));
      var res = await Task.WhenAll(tasks);
      return res;
    }

    protected async Task<RpcClientResponse> CallRequest(string service, string endpoint, object payload)
    {
      var req = new RpcClientRequest { Service = service, Payload = payload };
      var res = await Send(endpoint, req.ToArray());
      if (res.Error != null) throw new RpcException(res);
      return res;
    }

    protected async Task<LookupValue> ResolveService(string service)
    {
      long now = DateTime.Now.Ticks;
      bool hasKey = Lookups.ContainsKey(service);
      if (!hasKey || now - Lookups[service].LastUpdated > CacheAge)
      {
        var res = await Link.Lookup(service);
        if (hasKey)
        {
          Lookups[service].Endpoints = res;
          Lookups[service].LastUpdated = now;
        }
        else
        {
          Lookups.Add(service, new LookupValue { LastUpdated = now, Endpoints = res });
        }
      }

      return Lookups[service];
    }

    public static T ParseRpcResponseData<T>(RpcClientResponse response)
    {
      return JsonConvert.DeserializeObject<T>(response.Data);
    }

    protected abstract Task<RpcClientResponse> Send(string endpoint, Object[] req);
  }
}
