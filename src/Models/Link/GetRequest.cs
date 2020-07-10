using Newtonsoft.Json;

namespace Grenache.Models.Link
{
  public class GetRequest : RequestBase
  {
    [JsonProperty("data")]
    public string Data { get; set; }
  }
}
