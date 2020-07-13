# [Grenache](https://github.com/bitfinexcom/grenache) C# implementation

<img src="https://raw.githubusercontent.com/bitfinexcom/grenache-nodejs-http/master/logo.png" width="15%" />

Grenache is a micro-framework for connecting microservices. Its simple and optimized for performance.

Internally, Grenache uses Distributed Hash Tables (DHT, known from Bittorrent) for Peer to Peer connections. You can find more details how Grenche internally works at the [Main Project Homepage](https://github.com/bitfinexcom/grenache)

 - [Setup](#setup)
 - [Examples](#examples)

## Setup

### Install
...

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
