# [Grenache](https://github.com/bitfinexcom/grenache) C# implementation

<img src="./packageIcon.png" width="15%" />

Grenache is a micro-framework for connecting microservices. Its simple and optimized for performance.

Internally, Grenache uses Distributed Hash Tables (DHT, known from Bittorrent) for Peer to Peer connections. You can find more details how Grenche internally works at the [Main Project Homepage](https://github.com/bitfinexcom/grenache)

 - [Setup](#setup)
 - [Examples](#examples)

## Setup

### Install

As a first step you need to add github auth to nuget. First go to https://github.com/settings/tokens/new and make sure you've checked `read:packages` option. Once you do that go to your project directory and add source for the repo in nuget:

```bash
dotnet nuget add source https://nuget.pkg.github.com/bitfinexcom/index.json ---name github --username <YOUR_GITHUB_USERNAME> --password <YOUR_GENERATED_TOKEN> --store-password-in-clear-text
```

After you've added the repo to your env then you can install the grenache lib by running this cmd:
```bash
dotnet add package Grenache --version <PACKAGE_VER> --source github
```

### Other Requirements

Install `Grenache Grape`: https://github.com/bitfinexcom/grenache-grape:

```bash
npm i -g grenache-grape
```

```
// Start 2 Grapes
grape --dp 20001 --aph 30001 --bn '127.0.0.1:20002'
grape --dp 20002 --aph 40001 --bn '127.0.0.1:20001'
```

### Examples

#### RPC Server / Client

This RPC Server example introduces a service called `RpcPingService` within the overlay network. The server listens for a specific method indicated in the payload of the incoming requests. Here is how the interaction unfolds:

- **Client Action:** Sends a request with payload `hello`.
- **Server Response:** Responds with `world`.

The server handles the incoming requests in the `RpcPingService` class by identifying the method specified in the request body using the following template:

```csharp
var rpcRes = await client.Request("rpc_ping",
    """
    {
        "action": "Greet",
        "args": [{ "message": "hello" }]
    }
    """);


Internally the DHT is asked for the IP of the server and then the
request is done as Peer-to-Peer request via websockets.

**Grape:**

```bash
grape --dp 20001 --aph 30001 --bn '127.0.0.1:20002'
grape --dp 20002 --aph 40001 --bn '127.0.0.1:20001'
```

**Server:**

```csharp
using Grenache;
using Grenache.Models.PeerRPC;
...

Link link = new("http://127.0.0.1:30001");
var pingService = new RpcPingService();
var actionHandler = new RpcActionHandler(pingService.GetType().Assembly);
_server = new HttpPeerRPCServer(link, 10000);
_server.AddRequestHandler((req, res) =>
{
  var resultDelegate = actionHandler.HandleAction(req.Payload);
  var data = resultDelegate(pingService);
  res.Invoke(new RpcServerResponse { RId = req.RId, Data = data });
});
var started = await _server.Listen("rpc_ping", 7070);
if (!started) throw new Exception("Couldn't start the server!");
Console.WriteLine("Server started!");

CloseHandler();

await _server.ListenerTask; // used to keep the app always running

```

**Client:**

```csharp
using Grenache;
...

Link link = new("http://127.0.0.1:30001");
HttpPeerRPCClient client = new(link);

Console.WriteLine("Request: rpc_ping hello");
var rpcRes = await client.Request("rpc_ping",
    """
    {
        "action": "Greet",
        "args": [{ "message": "hello" }]
    }
    """);
Console.WriteLine("Response: " + rpcRes.Data);

Console.WriteLine("Request: rpc_ping Action = \"Point\", Args = [new {point = new { x = \"10\" }}] Multiplies by 5");
rpcRes = await client.Request("rpc_ping", new RpcRequest
{
    Action = "Point", 
    Args = [new {point = new { x = 10 }}]
});
Console.WriteLine("Response: " + rpcRes.Data);

Console.WriteLine("Request: rpc_ping Action = \"Point2D\", Args = [new {point = new Point2D { X = \"5\", Y = \"10\" }}] Multiplies by 3");
rpcRes = await client.Request("rpc_ping", new RpcRequest
{
    Action = "Point2D",
    Args = [ new {point = new Point2D { X = 5, Y = 10 }}]
});
Console.WriteLine("Response: " + rpcRes.Data);

var point = PeerRPCClient.ParseRpcResponseData<Point2D>(rpcRes);
Console.WriteLine($"Parsed object: x: {point.X}, y: {point.Y}");

Console.WriteLine("Map Request: rpc_ping test map");
var mapRpcRes = await client.Map("rpc_ping", "test map");
Console.WriteLine("Mapped Response: " + string.Join(",", mapRpcRes.Select(x => x.Data)));

```

### Testing

In order to run the unit tests first go to Grenache.Test directory. Once there install pm2 dependency via:
```bash
npm i
```

After that simply run:
```bash
./Grenache.Test/test.sh
```

## Licensing
Licensed under Apache License, Version 2.0
