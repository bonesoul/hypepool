```
 ▄ .▄ ▄· ▄▌ ▄▄▄·▄▄▄ . ▄▄▄·            ▄▄▌  
██▪▐█▐█▪██▌▐█ ▄█▀▄.▀·▐█ ▄█▪     ▪     ██•  
██▀▐█▐█▌▐█▪ ██▀·▐▀▀▪▄ ██▀· ▄█▀▄  ▄█▀▄ ██▪  
██▌▐▀ ▐█▀·.▐█▪·•▐█▄▄▌▐█▪·•▐█▌.▐▌▐█▌.▐▌▐█▌▐▌
▀▀▀ ·  ▀ • .▀    ▀▀▀ .▀    ▀█▄▀▪ ▀█▄▀▪.▀▀▀
```

**hypepool** is an rock-solid & platform-agnostic pool server which is a next-gen successor of [CoiniumServ](https://github.com/bonesoul/CoiniumServ) running on [dotnet core 2.0](https://github.com/dotnet/core) + [libuv](https://github.com/libuv/libuv).

### donations

**hypepool** is an open-source project. Your donations will be a great help & motivation for the development of the project.

```
* BTC: 1HTEVaWg8jp7HehfujrqupduLvZvX16Jih
* ETH: 0x61aa3e0709e20bcb4aedc2607d4070f1db72e69b
* DASH: XwnjnCoTb7v3FgDNrHvXufiLmfr5P5ZpEo
* DOGE: DE1JdC2wTeQERjnM25veQzd8CzB6vjm1JF
* LTC: LV4tiNmt2UuWphzBJSb1XABzufEWQHJfhJ
```

### platforms

can run on any platform that dotnet core 2.0 is [available](https://github.com/dotnet/core/blob/master/platforms.md).
* linux
* windows
* osx
* even more; Raspberry Pi, arm32

for list of supported operating systems [check here](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md).

### aims

* rock-solid code.
* being platform-agnostic.
* aiming high performance.
* being extremely efficient.

### requirements

* node 8+
* dotnet core 2.0
* mongodb

### technology

* dotnet core 2.0.
* libUV for high-performance sockets.

### features

- [ ] high performance sockets & stratum server.
- [ ] efficient share processor.
- [ ] rock solid payment processor.      
- [ ] superior extensiblity.
- [ ] vardiff support.
- [ ] banning support.
- [ ] full-stack API.
- [ ] multiple pools & ports support.
- [ ] multiple coin daemon connections

### supported blockchains

- [ ] bitcoin and clones.
- [ ] ethereum
- [ ] dash
- [ ] monero
- [ ] zcash

keep in mind that you can easly implement support for other blockchains too.

### web frontend

To support the project, we'll be stripping out full-featured web frontend as a standalone project which can be purchased to support the development. Still, the user can implement his own web-frontend using the API.

### status

development ongoing.

### installation

* install node 8+ (https://nodejs.org/en/download/)
* `npm install -g grunt-cli && npm install`
* `git clone git@github.com:bonesoul/hypepool.git`
* `grunt`
