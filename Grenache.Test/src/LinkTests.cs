using Xunit;
using Grenache.Utils;
using System.Net.Http;

namespace Grenache.Test
{
  public class LinkTests
  {
    private readonly Link link;

    public LinkTests()
    {
      HttpUtil.SetClient(new HttpClient());
      link = new Link("http://127.0.0.1:30005");
    }

    [Fact]
    public async void PerformsAnnounce()
    {
      var res = await link.Announce("my_unit_service", 30010);
      Assert.True(res, "Couldn't announce service");
    }

    [Fact]
    public async void PerformsLookup()
    {
      await link.Announce("my_unit_service", 30010);
      var successRes = await link.Lookup("my_unit_service");
      var emptyRes = await link.Lookup("my_unit_service12345");
      Assert.True(successRes.Length > 0, "Couldn't find my_unit_service");
      Assert.True(emptyRes.Length == 0, "Found my_unit_service12345 which is unexpected");
    }

    [Fact]
    public async void PerformsPutAndGet()
    {
      var hash = await link.Put("my_value");
      Assert.True(hash.Length > 0);
      var value = await link.Get(hash);
      Assert.True(value.Value == "my_value", "Value is not equal to my_value");

      hash = await link.Put(new { X = 33 });
      Assert.True(hash.Length > 0);
      value = await link.Get(hash);
      Assert.True(value.Value == "{\"X\":33}", "Value is not equal to {X:33}");
    }
  }
}
