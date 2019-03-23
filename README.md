# jaytwo.ejson

A .NET Core version of [Shoppify's ejson](https://github.com/Shopify/ejson).  I read their [blog post](https://engineering.shopify.com/blogs/engineering/secrets-at-shopify-introducing-ejson)  and thought it was a good idea.  But I'm a .NET developer and mainly work on Windows, so installing with `gem` or a `.deb` isn't as  practical in my world.  Using through their go source files as an example, I wrote this in C#.

I loved the ejson approach because:

* **\[Simple\]** It's just json
* **\[Secure\]** It uses as asymmetric encryption (public key to encrypt, private key to decrypt)
  * Safe for source control, developer friendly

From the [ejson readme](https://github.com/Shopify/ejson/blob/master/README.md): 

> `ejson` is a utility for managing a collection of secrets in source control. The
secrets are encrypted using [public
key](http://en.wikipedia.org/wiki/Public-key_cryptography), [elliptic
curve](http://en.wikipedia.org/wiki/Elliptic_curve_cryptography) cryptography
([NaCl](http://nacl.cr.yp.to/) [Box](http://nacl.cr.yp.to/box.html):
[Curve25519](http://en.wikipedia.org/wiki/Curve25519) +
[Salsa20](http://en.wikipedia.org/wiki/Salsa20) +
[Poly1305-AES](http://en.wikipedia.org/wiki/Poly1305-AES)). Secrets are
collected in a JSON file, in which all the string values are encrypted. Public
keys are embedded in the file, and the decrypter looks up the corresponding
private key from its local filesystem.

_(*Note: I have implemented additional private key providers.  So although the CLI works the same way as the original, the ASP.NET configuration can load encryption keys from other sources, such as environment variables.)_

This has been implemented as a [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), as well as a configuration library
for use in ASP.NET Core.

## For ASP.NET Configuration

### Installation

Add the NuGet package in your ASP.NET Core project:

```
PM> Install-Package jaytwo.ejson.Configuration
```

### Usage

```cs
Configuration = config
    .AddEJsonFile("appsettings.ejson")
    .Build();
```

## Command Line

### Installation

To install as a [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) (requires [NET Core 2.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.1))

```
dotnet tool install -g jaytwo.ejson.CommandLine
```

### Normal Usage

```bash
# To view help:
ejson --help

# Generate a keypair without persisting to disk:
ejson keygen

# Generate a keypair and write to disk:
ejson keygen -w

# To encrypt a file:
ejson encrypt foo.ejson

# To display the decrypted contents of a file:
ejson encrypt foo.ejson

# To save the decrypted contents of a file:
ejson encrypt encrypted.ejson -o decrypted.json
```

## Implementation Notes

`ejson` takes care of the immediate problem with storing secrets in plain text at rest or in source control, but you still need to securely provision the private key to be accessible by the application at runtime.  There are a few options for this, some more secure than others.  Shopify's outlines their solution to in their awesome [Secrets at Shopify - Introducing EJSON](https://engineering.shopify.com/blogs/engineering/secrets-at-shopify-introducing-ejson) blog post.  Their approach is pretty bespoke, involving re-encrypting the files with a shared _infrastructure_ upon deploy and a custom docker init process. 

### Sourcing Private Keys From the Filesystem

You can throw the private key on the filesystem at a one of the default locations, or you can specify a custom location.  This applies to the both the ASP.NET configuration implementation as well as the CLI.

Default filesystem locations:

* Windows: `%USERPROFILE%/.ejson/keys` (e.g. `C:\Users\johndoe\.ejson\keys`)
* OSX: `%HOME%/.ejson/keys` (e.g. `/Users/johndoe/.ejson/keys`)
* Other: `/opt/ejson/keys`
* Custom: Set environment variable `EJSON_KEYDIR`

Windows and OSX defaults are user scoped just because I assume you'll be working on developing on Windows or OSX and deploying to linux.  It will attempt to _find_ keys in any of those locations, but it will _save_ keys only to the default _(Note: the original `ejson` looked at `/opt/ejson/keys` on all platforms)_, unless overridden with the `--keyDir` CLI option or the `EJSON_KEYDIR` environment variable.

### Sourcing Private Keys From the Environment Variables

Somtiemes it's easier to throw the private key into an environment variable.  The name of the variable is the public key prefixed by `EJK_`.  The value will be the privat ekey.

```bash
# public key:  3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52
# private key: edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd

export EJK_3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52=edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd
```

The prefix can also be customized by setting the environment variable `EJSON_KEYPREFIX`.

## Encryption Library

Encryption is hard to get right.  Like any good developer, I'm leveraging a library that does it for me.  Encryption is done by [libsodium-core](https://github.com/tabrath/libsodium-core/).

* `libsodium-core` is a .NET Core version of [libsodium-net](https://github.com/adamcaudill/libsodium-net)
* `libsodium-net` is a wrapper around [libsodium](https://github.com/jedisct1/libsodium) for .NET
* `libsodium` is a portable, cross-compilable, installable, packageable fork of [NaCl](http://nacl.cr.yp.to/)
* `NaCl` (Networking and Cryptography library) is a great library that does all hard things
