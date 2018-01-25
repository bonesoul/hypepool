<h1 align="center">hypepool</h1>

<div align="center">
  :rocket::zap::sparkles: 
</div>
<div align="center">
  <strong>mining pool server</strong>
</div>
<div align="center">
  A rock-solid & platform-agnostic pool server
</div>

<br />

<div align="center">
  <!-- Build Status -->
  <a href="https://circleci.com/gh/bonesoul/hypepool/tree/develop">
    <img src="https://circleci.com/gh/bonesoul/hypepool/tree/develop.svg?style=svg&circle-token=2de3caa01614f309bedc9a2e6f986008e9e7c3af"
      alt="Build Status" />
  </a>
  <!-- Documentation -->
  <a href="https://readthedocs.org/projects/coiniumserv/?badge=latest">
    <img src="https://readthedocs.org/projects/hypepool-book/badge/?version=latest"
      alt="Build Status" />
  </a> 
</div>

<div align="center">
  <h3>
    <a href="#">
      Website
    </a>
    <span> | </span>
    <a href="#">
      Handbook
    </a>
    <span> | </span>
    <a href="https://github.com/bonesoul/hypepool/blob/develop/CONTRIBUTING.md">
      Contributing
    </a>
  </h3>
</div>

<div align="center">
  <sub>Built with ❤︎ by
  <a href="https://github.com/bonesoul">Hüseyin Uslu</a> and
  <a href="https://github.com/bonesoul/hypepool/graphs/contributors">
    contributors
  </a>
</div>
 
 ## hypepool

**hypepool** is an rock-solid & platform-agnostic pool server which is a next-gen successor of [CoiniumServ](https://github.com/bonesoul/CoiniumServ) running on [dotnet core 2.0](https://github.com/dotnet/core) + [libuv](https://github.com/libuv/libuv).

![hypepool](https://image.ibb.co/cLaDKG/hypepool.png)

### donations

**hypepool** is an open-source project. Your donations will be a great help & motivation for the development of the project.

```
ethereum:0x61aa3e0709e20bcb4aedc2607d4070f1db72e69b
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

* install dotnet core 2.0.5.
* install node 8+ (https://nodejs.org/en/download/)
* `npm install -g grunt-cli && npm install`
* `git clone git@github.com:bonesoul/hypepool.git`

finally run;
* `grunt build`
