using System.Threading.Tasks;

namespace Grenache.Interfaces;

public interface IRPCServer
{
  Task<bool> Listen(string service, int port);
  Task Close();
  void AddRequestHandler(RpcRequestHandler handler);
  void RemoveRequestHandler(RpcRequestHandler handler);
}
