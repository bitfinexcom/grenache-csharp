using System.Text.Json.Serialization;

namespace Grenache.Models.Link
{
  public class GetResponse
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("v")]
    public string Value { get; set; }
  }
}
