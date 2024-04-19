using System.Text.Json.Serialization;

namespace Grenache.Models.Link
{
  public class PutRequest : RequestBase
  {
    [JsonPropertyName("data")]
    public PutRequestData Data { get; set; }
  }

  public class PutRequestData
  {
    [JsonPropertyName("v")]
    public string Value { get; set; }
  }
}
