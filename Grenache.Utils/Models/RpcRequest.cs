namespace Grenache.Utils.Models;

public class RpcRequest
{
  public string Action { get; set; }
  public object[] Args { get; set; }
}
