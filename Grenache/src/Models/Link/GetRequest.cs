using System.Text.Json.Serialization;

namespace Grenache.Models.Link
{
  public class GetRequest : RequestBase
  {
    [JsonPropertyName("data")]
    public string Data { get; set; }
  }
}
