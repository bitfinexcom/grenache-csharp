using System;

namespace Grenache.Models.PeerRPC
{
  public class RpcException : Exception
  {
    public Guid RId { get; protected set; }
    public string RpcData { get; protected set; }

    public RpcException(string message, Guid rId, string rpcData) : base(message)
    {
      RId = rId;
      RpcData = rpcData;
    }

    public RpcException(RpcClientResponse response) : this(response.Error, response.RId, response.Data)
    {

    }
  }
}
