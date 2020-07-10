using System;
using System.Collections.Generic;
using System.Net.Http;
using Grenache.Models;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Grenache
{
  public class Link
  {
    protected readonly string grape;
    protected static readonly HttpClient client = new HttpClient(); // shared between all instances

    public Link(string grape)
    {
      this.grape = grape;
    }

    public async Task<IReadOnlyList<string>> Lookup(string service)
    {
      var req = new LookupRequest { Data = service };
      var res = await PostRequest($"{grape}/lookup", req);
      return JsonConvert.DeserializeObject<List<string>>(res);
    }

    public async Task<bool> Announce(string service, int port)
    {
      var req = new AnnounceRequest { Service = service, Port = port };
      var res = await PostRequest($"{grape}/announce", req);
      return JsonConvert.DeserializeObject<int>(res) == 1;
    }

    public async Task<string> Put(Object value)
    {
      string serialized = JsonConvert.SerializeObject(value);
      var res = await Put(serialized);
      return res;
    }

    public async Task<string> Put(string value)
    {
      var req = new PutRequest { Data = new PutRequestData { Value = value } };
      var res = await PostRequest($"{grape}/put", req);
      return JsonConvert.DeserializeObject<string>(res);
    }

    public async Task<GetResponse> Get(string hash)
    {
      var req = new GetRequest { Data = hash };
      var res = await PostRequest($"{grape}/get", req);
      return JsonConvert.DeserializeObject<GetResponse>(res);
    }

    protected static async Task<string> PostRequest<T>(string url, T req)
    {
      var json = JsonConvert.SerializeObject(req);
      var body = new StringContent(json, Encoding.UTF8, "application/json");
      var res = await client.PostAsync(url, body);
      res.EnsureSuccessStatusCode();

      var data = await res.Content.ReadAsStringAsync();
      return data;
    }
  }
}
