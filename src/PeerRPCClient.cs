using Grenache.Models.PeerRPC;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Grenache
{
  public abstract class PeerRPCClient
  {
    protected readonly Link link;
    protected readonly long cacheAge;
    protected Dictionary<string, LookupValue> lookups;

    public PeerRPCClient(Link link, long cacheAge = 120 * 1000)
    {
      this.link = link;
      this.cacheAge = cacheAge;
      this.lookups = new Dictionary<string, LookupValue>();
    }

    public async Task<RpcResponse> Request(string service, object payload)
    {
      var lookup = await ResolveService(service);
      if (lookup.Endpoints.Length == 0) throw new Exception("ERR_GRAPE_LOOKUP_EMPTY");

      var res = await CallRequest(service, lookup.Endpoints[0], payload);
      return res;
    }

    public async Task<RpcResponse[]> Map(string service, object payload)
    {
      var lookup = await ResolveService(service);
      if (lookup.Endpoints.Length == 0) throw new Exception("ERR_GRAPE_LOOKUP_EMPTY");

      var tasks = lookup.Endpoints.Select(ep => CallRequest(service, ep, payload));
      var res = await Task.WhenAll(tasks);
      return res;
    }

    protected async Task<RpcResponse> CallRequest(string service, string endpoint, object payload)
    {
      var req = new RpcRequest { Service = service, Payload = payload };
      var res = await Send(endpoint, req.ToArray());
      if (res.Error != null) throw new RpcException(res);
      return res;
    }

    protected async Task<LookupValue> ResolveService(string service)
    {
      long now = DateTime.Now.Ticks;
      bool hasKey = lookups.ContainsKey(service);
      if (!hasKey || now - lookups[service].LastUpdated > cacheAge)
      {
        var res = await link.Lookup(service);
        if (hasKey)
        {
          lookups[service].Endpoints = res;
          lookups[service].LastUpdated = now;
        }
        else
        {
          lookups.Add(service, new LookupValue { LastUpdated = now, Endpoints = res });
        }
      }

      return lookups[service];
    }

    public static T ParseRpcResponseData<T> (RpcResponse response)
    {
      return JsonConvert.DeserializeObject<T> (response.Data);
    }

    protected abstract Task<RpcResponse> Send(string endpoint, Object[] req);
  }
}
