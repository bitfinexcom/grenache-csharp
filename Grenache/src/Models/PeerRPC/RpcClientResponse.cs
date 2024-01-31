using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcClientResponse
  {
    public Guid RId { get; set; }
    public string Error { get; set; }
    public string Data { get; set; }

    public static RpcClientResponse FromArray(object[] arr)
    {
      return new RpcClientResponse
      {
        RId = Guid.Parse(arr[0].ToString()),
        Error = arr[1]?.ToString(),
        Data = arr[2]?.ToString()
      };
    }
  }
}
