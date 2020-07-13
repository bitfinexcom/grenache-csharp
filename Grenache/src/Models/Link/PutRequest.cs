using Newtonsoft.Json;

namespace Grenache.Models.Link
{
  public class PutRequest : RequestBase
  {
    [JsonProperty("data")]
    public PutRequestData Data { get; set; }
  }

  public class PutRequestData
  {
    [JsonProperty("v")]
    public string Value { get; set; }
  }
}
