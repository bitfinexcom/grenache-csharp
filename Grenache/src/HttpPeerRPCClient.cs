using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Grenache.Utils;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public class HttpPeerRPCClient(Link link, long cacheAge = 120 * 1000, bool isSecure = false)
    : PeerRPCClient(link, cacheAge)
  {
    protected bool IsSecure { get; set; } = isSecure;

    protected override async Task<RpcClientResponse> Send(string endpoint, object[] req)
    {
      var url = $"{(IsSecure ? "https" : "http")}://{endpoint}";
      var headers = new Dictionary<string, string>
      {
        ["_gr"] = JsonSerializer.Serialize(new[] { req[0], req[1] }) // [rid, service]
      };

      var res = await HttpUtil.PostRequestAsync<object[], object[]>(url, req, headers);
      return RpcClientResponse.FromArray(res);
    }
  }
}
