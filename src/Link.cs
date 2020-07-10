using Grenache.Models.Link;
using Grenache.Utils;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Grenache
{
  public class Link
  {
    protected readonly string grape;

    public Link(string grape)
    {
      this.grape = grape;
    }

    public async Task<string[]> Lookup(string service)
    {
      var req = new LookupRequest { Data = service };
      var res = await HttpUtil.PostRequest($"{grape}/lookup", req);
      return JsonConvert.DeserializeObject<string[]>(res);
    }

    public async Task<bool> Announce(string service, int port)
    {
      var req = new AnnounceRequest { Service = service, Port = port };
      var res = await HttpUtil.PostRequest($"{grape}/announce", req);
      return JsonConvert.DeserializeObject<int>(res) == 1;
    }

    public async Task<string> Put(object value)
    {
      string serialized = JsonConvert.SerializeObject(value);
      var res = await Put(serialized);
      return res;
    }

    public async Task<string> Put(string value)
    {
      var req = new PutRequest { Data = new PutRequestData { Value = value } };
      var res = await HttpUtil.PostRequest($"{grape}/put", req);
      return JsonConvert.DeserializeObject<string>(res);
    }

    public async Task<GetResponse> Get(string hash)
    {
      var req = new GetRequest { Data = hash };
      var res = await HttpUtil.PostRequest($"{grape}/get", req);
      return JsonConvert.DeserializeObject<GetResponse>(res);
    }
  }
}
