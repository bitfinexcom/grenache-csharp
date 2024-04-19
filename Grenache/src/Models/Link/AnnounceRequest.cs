using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grenache.Models.Link
{
  public class AnnounceRequestConverter : JsonConverter<AnnounceRequest>
  {
    public override AnnounceRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      using var doc = JsonDocument.ParseValue(ref reader);
      var root = doc.RootElement;

      var req = new AnnounceRequest
      {
        RId = Guid.Parse(root.GetProperty("rid").GetString() ?? throw new InvalidOperationException()),
        Service = root.GetProperty("data")[0].GetString(),
        Port = root.GetProperty("data")[1].GetInt32()
      };

      return req;
    }

    public override void Write(Utf8JsonWriter writer, AnnounceRequest value, JsonSerializerOptions options)
    {
      writer.WriteStartObject();
      writer.WriteString("rid", value.RId.ToString());

      writer.WriteStartArray("data");
      writer.WriteStringValue(value.Service);
      writer.WriteNumberValue(value.Port);
      writer.WriteEndArray();

      writer.WriteEndObject();
    }
  }

  [JsonConverter(typeof(AnnounceRequestConverter))]
  public class AnnounceRequest : RequestBase
  {
    public string Service { get; set; }
    public int Port { get; set; }
  }
}
