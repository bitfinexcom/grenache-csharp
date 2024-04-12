using System.Threading.Tasks;
using Grenache.Utils;
using Grenache.Models.Link;
using System.Text.Json;

namespace Grenache
{
  public class Link(string grape)
  {
    protected string Grape { get; } = grape;

    public async Task<string[]> Lookup(string service)
    {
      var req = new LookupRequest { Data = service };
      return await HttpUtil.PostRequestAsync<LookupRequest, string[]>($"{Grape}/lookup", req);
    }

    public async Task<bool> Announce(string service, int port)
    {
      var req = new AnnounceRequest { Service = service, Port = port };
      var response = await HttpUtil.PostRequestAsync<AnnounceRequest, int>($"{Grape}/announce", req);
      return response == 1;
    }

    public async Task<string> Put(object value)
    {
      var req = new PutRequest { Data = new PutRequestData { Value = JsonSerializer.Serialize(value) } };
      return await HttpUtil.PostRequestAsync<PutRequest, string>($"{Grape}/put", req);
    }

    public async Task<string> Put(string value)
    {
      var req = new PutRequest { Data = new PutRequestData { Value = value } };
      return await HttpUtil.PostRequestAsync<PutRequest, string>($"{Grape}/put", req);
    }

    public async Task<GetResponse> Get(string hash)
    {
      var req = new GetRequest { Data = hash };
      return await HttpUtil.PostRequestAsync<GetRequest, GetResponse>($"{Grape}/get", req);
    }
  }
}
