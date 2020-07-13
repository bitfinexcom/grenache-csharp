using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcServerRequest
  {
    public Guid RId { get; set; } = Guid.NewGuid();
    public string Service { get; set; }
    public string Payload { get; set; }

    public static RpcServerRequest FromArray(object[] arr)
    {
      return new RpcServerRequest
      {
        RId = Guid.Parse(arr[0].ToString()),
        Service = arr[1].ToString(),
        Payload = arr[2] != null ? arr[2].ToString() : null
      };
    }
  }
}
