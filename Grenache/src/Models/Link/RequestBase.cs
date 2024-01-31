using System;
using System.Text.Json.Serialization;

namespace Grenache.Models.Link
{
  public class RequestBase
  {
    [JsonPropertyName("rid")]
    public Guid RId { get; set; } = Guid.NewGuid();
  }
}
