# jaytwo.ejson

A .NET Core version of [Shoppify's ejson](https://github.com/Shopify/ejson).

I read their [blog post](https://engineering.shopify.com/blogs/engineering/secrets-at-shopify-introducing-ejson) and thought it was a good idea.  But I'm a .NET developer and mainly work on Windows, so installing with `gem` or a `.deb` isn't as practical in my world.

Using through their go source files as an example, I wrote this in C#.

## Installation

To install as a [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) (requires [NET Core 2.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.1))

```
dotnet tool install -g jaytwo.ejson.CommandLine
```

## Normal Usage

To view help:

```bash
ejson --help
```

## Encryption Library

Encryption is hard to get right.  Like any good developer, I'm leveraging a library that does it for me.  Encryption is done by [libsodium-core](https://github.com/tabrath/libsodium-core/).

* `libsodium-core` is a .NET Core version of [libsodium-net](https://github.com/adamcaudill/libsodium-net)
* `libsodium-net` is a wrapper around [libsodium](https://github.com/jedisct1/libsodium) for .NET
* `libsodium` is a portable, cross-compilable, installable, packageable fork of [NaCl](http://nacl.cr.yp.to/)
* `NaCl` (Networking and Cryptography library) is a great library that does all hard things
