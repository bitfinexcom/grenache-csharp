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
dotnet nuget add source https://nuget.pkg.github.com/bitfinexcom/index.json -n bitfinexcom-github -u <YOUR_USERNAME> -p <YOUR_GENERATED_TOKEN> --store-password-in-clear-text
```

After you've added the repo to your env then you can install the grenache lib by running this cmd:
```bash
dotnet add package Grenache --version <PACKAGE_VER> --source bitfinexcom-github
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
...

### Testing

In order to run the unit tests simply run the cmd below, it will start all grapes and run the necessary tests.

```bash
./Grenache.Test/test.sh
```

## Licensing
Licensed under Apache License, Version 2.0
