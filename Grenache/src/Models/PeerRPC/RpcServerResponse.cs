using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcServerResponse
  {
    public Guid RId { get; set; }
    public string Error { get; set; }
    public object Data { get; set; }

    public object[] ToArray()
    {
      return new object[] { RId, Error, Data };
    }
  }
}
