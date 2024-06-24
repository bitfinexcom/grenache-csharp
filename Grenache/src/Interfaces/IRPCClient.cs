using System.Text.Json;
using System.Threading.Tasks;
using Grenache.Models.PeerRPC;

namespace Grenache.Interfaces;

public interface IRPCClient
{
  Task<RpcClientResponse> Request(string service, object payload);
  Task<RpcClientResponse[]> Map(string service, object payload);

  static T ParseRpcResponseData<T>(RpcClientResponse response)
  {
    return JsonSerializer.Deserialize<T>(response.Data);
  }
}
