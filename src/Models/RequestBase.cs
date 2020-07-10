using System;
using Newtonsoft.Json;

namespace Grenache.Models
{
  public class RequestBase
  {
    [JsonProperty("rid")]
    public Guid RId { get; set; } = Guid.NewGuid();
  }
}
