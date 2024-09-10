namespace Grenache.Models;

public class RpcActionRequest
{
  public string Action { get; set; }
  public object[] Args { get; set; }
}
