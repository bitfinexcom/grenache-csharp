using System.Threading.Tasks;
using Newtonsoft.Json;
using Grenache.Utils;
using Grenache.Models.Link;

namespace Grenache
{
  public class Link
  {
    protected string Grape { get; set; }

    public Link(string grape)
    {
      Grape = grape;
    }

    public async Task<string[]> Lookup(string service)
    {
      var req = new LookupRequest { Data = service };
      var res = await HttpUtil.PostRequest($"{Grape}/lookup", req);
      return JsonConvert.DeserializeObject<string[]>(res);
    }

    public async Task<bool> Announce(string service, int port)
    {
      var req = new AnnounceRequest { Service = service, Port = port };
      var res = await HttpUtil.PostRequest($"{Grape}/announce", req);
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
      var res = await HttpUtil.PostRequest($"{Grape}/put", req);
      return JsonConvert.DeserializeObject<string>(res);
    }

    public async Task<GetResponse> Get(string hash)
    {
      var req = new GetRequest { Data = hash };
      var res = await HttpUtil.PostRequest($"{Grape}/get", req);
      return JsonConvert.DeserializeObject<GetResponse>(res);
    }
  }
}
