using Newtonsoft.Json;

namespace Grenache.Models
{
  public class LookupRequest : RequestBase
  {
    [JsonProperty("data")]
    public string Data { get; set; }
  }
}
