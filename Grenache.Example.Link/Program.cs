using System;
using System.Threading.Tasks;

namespace Grenache.Example.Link
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Utils.HttpUtil.SetClient(new System.Net.Http.HttpClient());

      var link = new Grenache.Link("http://127.0.0.1:30001");
      var announceRes = await link.Announce("rpc_test", 7070);
      Console.WriteLine("Service rpc_test announced: " + announceRes);

      var lookupRes = await link.Lookup("rpc_test");
      Console.WriteLine("Lookup result for service rpc_test:");
      foreach (var item in lookupRes)
      {
        Console.WriteLine($"\t{item}");
      }

      var putRes = await link.Put("string test");
      Console.WriteLine("Hash for data stored: " + putRes);
      var getRes = await link.Get(putRes);
      Console.WriteLine("Data retrieved from hash: " + getRes.Value);

      putRes = await link.Put(new { x = 55, y = "a" });
      Console.WriteLine("Hash for data stored: " + putRes);
      getRes = await link.Get(putRes);
      Console.WriteLine("Data retrieved from hash: " + getRes.Value);
    }
  }
}
