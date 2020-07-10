using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Grenache.Utils
{
  public static class HttpUtil
  {
    private static readonly HttpClient client = new HttpClient(); // shared between all instances

    public static HttpClient GetClient { get => client; }

    public static async Task<string> PostRequest<T>(string url, T req)
    {
      var json = JsonConvert.SerializeObject(req);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var res = await PostRequest(url, (HttpContent)content);
      return res;
    }

    public static async Task<string> PostRequest<T>(string url, T req, IDictionary<string, string> headers)
    {
      var json = JsonConvert.SerializeObject(req);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      foreach (var kv in headers)
        content.Headers.Add(kv.Key, kv.Value);

      var res = await PostRequest(url, (HttpContent)content);
      return res;
    }

    public static async Task<string> PostRequest(string url, HttpContent content)
    {
      var res = await client.PostAsync(url, content);
      res.EnsureSuccessStatusCode();

      var data = await res.Content.ReadAsStringAsync();
      return data;
    }
  }
}
