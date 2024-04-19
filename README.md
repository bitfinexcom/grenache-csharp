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

This RPC Server example announces a service called `rpc_test`
on the overlay network. When a request from a client is received,
it replies with `world`. It receives the payload `hello` from the
client.

The client sends `hello` and receives `world` from the server.

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

Link link = new Link("http://127.0.0.1:30001");
server = new HttpPeerRPCServer(link, 10000);

server.AddRequestHandler((req, res) =>
{
  Console.WriteLine("Payload: " + req.Payload);
  res.Invoke(new RpcServerResponse { RId = req.RId, Data = "world" });
});

var started = await server.Listen("rpc_test", 7070);
if (!started) throw new Exception("Couldn't start the server!");

await server.ListenerTask; // used to keep the app always running

```

**Client:**

```csharp
using Grenache;
...

Link link = new Link("http://127.0.0.1:30001");
HttpPeerRPCClient client = new HttpPeerRPCClient(link);

Console.WriteLine("Request: rpc_test hello");
var rpcRes = await client.Request("rpc_test", "hello");
Console.WriteLine("Response: " + rpcRes.Data);

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
