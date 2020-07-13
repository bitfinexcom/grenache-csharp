using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Grenache.Utils;
using Grenache.Models.PeerRPC;

namespace Grenache
{
  public class HttpPeerRPCClient : PeerRPCClient
  {
    protected bool IsSecure { get; set; }

    public HttpPeerRPCClient(Link link, long cacheAge = 120 * 1000, bool isSecure = false) : base(link, cacheAge)
    {
      IsSecure = isSecure;
    }

    protected override async Task<RpcClientResponse> Send(string endpoint, object[] req)
    {
      var url = $"{(IsSecure ? "https" : "http")}://{endpoint}";
      var headers = new Dictionary<string, string>();
      headers.Add("_gr", JsonConvert.SerializeObject(new object[] { req[0], req[1] })); // [rid, service]
      var json = await HttpUtil.PostRequest<object[]>(url, req, headers);

      var res = JsonConvert.DeserializeObject<object[]>(json);
      return RpcClientResponse.FromArray(res);
    }
  }
}
