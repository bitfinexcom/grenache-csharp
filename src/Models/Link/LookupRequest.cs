using Newtonsoft.Json;

namespace Grenache.Models.Link
{
  public class LookupRequest : RequestBase
  {
    [JsonProperty("data")]
    public string Data { get; set; }
  }
}
