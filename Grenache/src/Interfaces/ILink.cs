using System.Threading.Tasks;
using Grenache.Models.Link;

namespace Grenache.Interfaces;

public interface ILink
{
  Task<string[]> Lookup(string service);
  Task<bool> Announce(string service, int port);
  Task<string> Put(object value);
  Task<string> Put(string value);
  Task<GetResponse> Get(string hash);
}
