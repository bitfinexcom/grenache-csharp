using System;
using Newtonsoft.Json;

namespace Grenache.Models
{
  public class GetResponse
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("token")]
    public string Token { get; set; }
    [JsonProperty("v")]
    public string Value { get; set; }
  }
}
