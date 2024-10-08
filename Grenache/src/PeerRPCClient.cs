using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Grenache.Interfaces;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public abstract class PeerRPCClient(Link link, long cacheAge = 120 * 1000) : IRPCClient
  {
    protected Link Link { get; } = link;
    protected long CacheAge { get; } = cacheAge;
    protected ConcurrentDictionary<string, LookupValue> Lookups { get; } = new();

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
      var now = DateTime.Now.Ticks;
      var hasKey = Lookups.ContainsKey(service);

      if (!Lookups.TryGetValue(service, out var lookupValue) || now - lookupValue.LastUpdated > CacheAge)
      {
        var res = await Link.Lookup(service);
        var newValue = new LookupValue { LastUpdated = now, Endpoints = res };
        Lookups.AddOrUpdate(service, newValue, (key, oldValue) => newValue);
      }

      return Lookups[service];
    }

    protected abstract Task<RpcClientResponse> Send(string endpoint, object[] req);
  }
}
