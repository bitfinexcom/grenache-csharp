using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcRequest
  {
    public Guid RId { get; set; } = Guid.NewGuid();
    public string Service { get; set; }
    public object Payload { get; set; }

    public object[] ToArray()
    {
      return new object[] { RId, Service, Payload };
    }
  }
}
