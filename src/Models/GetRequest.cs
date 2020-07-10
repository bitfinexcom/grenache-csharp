using System;
using Newtonsoft.Json;

namespace Grenache.Models
{
  public class GetRequest : RequestBase
  {
    [JsonProperty("data")]
    public string Data { get; set; }
  }
}
