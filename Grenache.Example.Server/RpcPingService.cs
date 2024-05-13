namespace Grenache.Example.Server;

public class RpcPingService
{
  public static string Greet(string message)
  {
    if (message.Equals("hello", System.StringComparison.CurrentCultureIgnoreCase))
      return "world";
    return string.Empty;
  }

  public static Point Point(Point point) => new Point { X = point.X * 5 };

  public static Point2D Point2D(Point2D point) =>
    new Point2D
    {
      X = point.X * 3,
      Y = point.Y * 3
    };
}

public class Point
{
  public int X { get; init; }
}

public class Point2D
{
  public int X { get; init; }
  public int Y { get; init; }
}
