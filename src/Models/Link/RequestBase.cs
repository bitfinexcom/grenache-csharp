using System;
using Newtonsoft.Json;

namespace Grenache.Models.Link
{
  public class RequestBase
  {
    [JsonProperty("rid")]
    public Guid RId { get; set; } = Guid.NewGuid();
  }
}
