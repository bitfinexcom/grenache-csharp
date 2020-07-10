using Grenache.Models.PeerRPC;
using Grenache.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace Grenache
{
  public class HttpPeerRPCClient : PeerRPCClient
  {
    protected readonly bool isSecure;
    public HttpPeerRPCClient(Link link, long cacheAge = 120 * 1000, bool isSecure = false) : base(link, cacheAge)
    {
      this.isSecure = isSecure;
    }

    protected override async Task<RpcResponse> Send(string endpoint, object[] req)
    {
      var url = $"{(isSecure ? "https" : "http")}://{endpoint}";
      var headers = new Dictionary<string, string>();
      headers.Add("_gr", JsonConvert.SerializeObject(new object[] { req[0], req[1] })); // [rid, service]
      var json = await HttpUtil.PostRequest<object[]>(url, req, headers);

      var res = JsonConvert.DeserializeObject<object[]>(json);
      return RpcResponse.FromArray(res);
    }
  }
}
