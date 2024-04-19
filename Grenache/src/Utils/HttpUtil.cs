using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Grenache.Utils
{
  public static class HttpUtil
  {
    private static HttpClient _client;

    public static void SetClient(HttpClient httpClient)
    {
      _client = httpClient;
    }

    public static async Task<TRes> PostRequestAsync<TReq, TRes>(string url, TReq req, IDictionary<string, string> headers = null)
    {
      var request = new HttpRequestMessage(HttpMethod.Post, url)
      {
        Content = JsonContent.Create(req)
      };

      // Add headers if they are provided
      if (headers != null)
      {
        foreach (var header in headers)
        {
          request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
      }

      var response = await _client.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var rawRes = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<TRes>(rawRes);
    }
  }
}
