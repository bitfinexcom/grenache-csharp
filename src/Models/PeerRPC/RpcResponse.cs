using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcResponse
  {
    public Guid RId { get; set; }
    public string Error { get; set; }
    public string Data { get; set; }

    public static RpcResponse FromArray(object[] arr)
    {
      return new RpcResponse
      {
        RId = Guid.Parse(arr[0].ToString()),
        Error = arr[1] != null ? arr[1].ToString() : null,
        Data = arr[2] != null ? arr[2].ToString() : null
      };
    }
  }
}
