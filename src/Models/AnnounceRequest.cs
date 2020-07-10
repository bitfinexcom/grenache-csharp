using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grenache.Models
{
  public class AnnounceRequestConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType == typeof(AnnounceRequest));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      JObject root = JObject.Load(reader);

      AnnounceRequest req = new AnnounceRequest();
      req.RId = Guid.Parse((string)root["rid"]);
      req.Service = (string)root["data"][0];
      req.Port = (int)root["data"][1];

      return req;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      AnnounceRequest req = (AnnounceRequest)value;

      JArray data = new JArray();
      data.Add(req.Service);
      data.Add(req.Port);

      JObject root = new JObject(
        new JProperty("rid", req.RId),
        new JProperty("data", data)
      );

      root.WriteTo(writer);
    }
  }

  [JsonConverter(typeof(AnnounceRequestConverter))]
  public class AnnounceRequest : RequestBase
  {
    public string Service { get; set; }
    public int Port { get; set; }
  }
}
