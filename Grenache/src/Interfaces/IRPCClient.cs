using System.Text.Json;
using System.Threading.Tasks;
using Grenache.Models.PeerRPC;

namespace Grenache.Interfaces;

public interface IRPCClient
{
  Task<RpcClientResponse> Request(string service, object payload);
  Task<RpcClientResponse[]> Map(string service, object payload);

  static T ParseRpcResponseData<T>(RpcClientResponse response, JsonSerializerOptions options = null)
  {
    if (typeof(T) == typeof(bool))
    {
      return (T)(object)bool.Parse(response.Data);
    }

    return JsonSerializer.Deserialize<T>(response.Data, options);
  }
}
