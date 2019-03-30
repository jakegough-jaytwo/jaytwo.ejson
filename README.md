# jaytwo.ejson

A .NET Core version of [Shoppify's ejson](https://github.com/Shopify/ejson).  I read their 
[blog post](https://engineering.shopify.com/blogs/engineering/secrets-at-shopify-introducing-ejson) 
and thought it was a good idea.  But I'm a .NET developer and mainly work on Windows, so installing 
with `gem` or a `.deb` isn't as  practical in my world.  Using their go source files as an example, 
I wrote this in C#.

I loved the ejson approach because:

* **\[Simple\]** It's just JSON
* **\[Secure\]** It uses as asymmetric encryption (public key to encrypt, private key to decrypt)
  * Safe for source control, developer friendly

From the [Shopify ejson readme](https://github.com/Shopify/ejson/blob/master/README.md): 

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

_(*Note: I have implemented additional private key providers. The .NET CLI version is compatible
with the original, but it also includes the ability to read keys from environment variables.  The 
ASP.NET Core configuration can load encryption keys from any `IConfiguration` ConfigSection.)_

This has been implemented as a [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), 
as well as a configuration library for use in ASP.NET Core.

## For ASP.NET Configuration

### Installation

Add the NuGet package in your ASP.NET Core web project:

```
PM> Install-Package jaytwo.ejson.AspNetCore.Configuration
```

### Usage

_By convention, ejson files have the `.ejson` extension.  However, Visual Studio doesn't know what `.ejson` is, and instead prefers `.json` files for  syntax highlighting and nesting in solution explorer._

`Program.cs` should configure the stuff that _can't_ go wrong.  In this case, `WebHost.CreateDefaultBuilder()` will load the `appsettings.json` files and configure logging.

```cs
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
}
```

`Startup.cs` will configure stuff that _can_ go wrong (like load secrets).  Luckily, at this point we 
can inject a configured `ILoggerFactory` (thanks to the `Program.cs`).  We can also inject the `IConfiguration` 
in the default state from `WebHost.CreateDefaultBuilder()` in `Program.cs`.

```cs
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configurationBeforeSecrets, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        _configuration = new ConfigurationBuilder()
            .AddConfiguration(configurationBeforeSecrets)
            .AddEjsonAppSecrets(env, loggerFactory)
            .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(x => _configuration);
        
        // ... and other stuff ...
    }
    
    // ... and other stuff ...
}
```

## Command Line

### Installation

To install as a [.NET Core Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) (requires [NET Core 2.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.1))

```
dotnet tool install -g jaytwo.ejson.CommandLine
```

### Normal Usage

```bash
# generate a key
$ ejson keygen -w
169f68900c6a9a7ee7fe1854197a372c96b728496aff5017dfd36e96df8d1a39

# use the public key
$ cat friends.ejson
{
  "_public_key": "169f68900c6a9a7ee7fe1854197a372c96b728496aff5017dfd36e96df8d1a39",
  "spoiler": "Rachel and Ross end up together"
}

# encrypt the file
$ ejson encrypt friends.ejson

$ cat friends.ejson
{
  "_public_key": "169f68900c6a9a7ee7fe1854197a372c96b728496aff5017dfd36e96df8d1a39",
  "spoiler": "EJ[1:Dq4cyj3nPNOml02xwYLXYKlrzT/V++kfDgO1gcazBBg=:+8Kb0VvhV1r6QXjG/1msevgPbqayGrnj:cMw+NVAOFE10mxnCqV0JVgoOnwm4jxae3Y9HADjClFxT0RaId03Vfpn7zBaWEDg=]"
}

# decrypt the file
$ ejson decrypt friends.ejson
{
  "_public_key": "169f68900c6a9a7ee7fe1854197a372c96b728496aff5017dfd36e96df8d1a39",
  "spoiler": "Rachel and Ross end up together"
}

$ ejson decrypt friends.ejson -o friends-decrypted.ejson
Saved to: friends-decrypted.ejson

$ cat friends-decrypted.ejson
{
  "_public_key": "169f68900c6a9a7ee7fe1854197a372c96b728496aff5017dfd36e96df8d1a39",
  "spoiler": "Rachel and Ross end up together"
}
```

## `ejson` Format (from the [Shopify ejson readme](https://github.com/Shopify/ejson/blob/master/README.md))

The ejson document format is simple, but there are a few points to be aware of:

1. It's just JSON.
2. There must be a key at the top level named `_public_key`, whose value is a 32-byte hex-encoded (i.e. 64 ASCII byte) public key as generated by `ejson keygen`.
3. Any string literal that isn't an object key will be encrypted by default (ie. in `{"a": "b"}`, `"b"` will be encrypted, but `"a"` will not.
4. Numbers, booleans, and nulls aren't encrypted.
5. If a key begins with an underscore, its corresponding value will not be encrypted. This is used to prevent the `_public_key` field from being encrypted, and is useful for implementing metadata schemes.
6. Underscores do not propagate downward. For example, in `{"_a": {"b": "c"}}`, `"c"` will be encrypted.

## Implementation Notes

`ejson` takes care of the immediate problem with storing secrets in plain text at rest or in source control, but 
you still need to securely provision the private key to be accessible by the application at runtime.  There are 
a few options for this, some more secure than others.  Shopify's outlines their solution in their awesome 
[Secrets at Shopify - Introducing EJSON](https://engineering.shopify.com/blogs/engineering/secrets-at-shopify-introducing-ejson) 
blog post.  Their approach is pretty bespoke, involving re-encrypting the files with a shared _infrastructure_ key 
upon deploy and a custom docker init process. 

### Sourcing Private Keys From the Filesystem

You can throw the private key on the filesystem at a one of the default locations, or you can specify a custom 
location.  This applies to the both the ASP.NET configuration implementation as well as the CLI.  (In the 
aforementioned blog post, Shopify distributes their private key files with Chef).

Default filesystem locations:

* Windows: `%USERPROFILE%\.ejson\keys` (e.g. `C:\Users\johndoe\.ejson\keys`)
* OSX: `$HOME/.ejson/keys` (e.g. `/Users/johndoe/.ejson/keys`)
* Other: `/opt/ejson/keys`
* Custom: Set environment variable `EJSON_KEYDIR`

Windows and OSX defaults are user-scoped just because I assume you'll be developing on Windows or OSX and deploying 
to linux.  It will attempt to _find_ keys in any of those locations, but it will _save_ keys only to the default 
_(Note: the original `ejson` looked at `/opt/ejson/keys` on all platforms)_, unless overridden with the `--keyDir` 
CLI option or the `EJSON_KEYDIR` environment variable.

### Sourcing Private Keys From the Environment Variables

The `EJSON_KEYDIR` environment variable tells the library where on the filesystem to look for a private key, but 
often that means the key is in plain text at rest on the disk.  What if you want to store the key itself in an 
environment variable?

Everything from container orchestration platforms to bare-metal IIS configuration to budget shared hosting providers 
support custom runtime environment variables.

The name of the variable is the public key prefixed by `EJK_`.  The value will be the private key.

```bash
# public key:  3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52
# private key: edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd

export EJK_3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52=edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd
```

The prefix can also be customized by setting the environment variable `EJSON_KEYPREFIX`.

## Encryption Notes

`ejson` is built on top of [NaCl](http://nacl.cr.yp.to/)'s Public Box messages (I found 
[PyNaCl's documentation](https://pynacl.readthedocs.io/en/stable/public/#nacl-public-box) the easiest to follow,
but feel free to go straight to the [source](https://nacl.cr.yp.to/box.html)).  

To encrypt (aka send message, create the box) you need:

* the public key of the receiver
* a random nonce
* the private key of an ephemeral keypair generated at the time of encryption

To decrypt (aka receive message, open the box) you need:

* the private key of the receiver
* the nonce generated by the sender
* the public key of the ephemeral keypair generated by the sender

In the `ejson` world, the sender is a developer, the receiver is the application at runtime.  The developer only needs the public key 
of the app, and the rest is generated at encryption time.  The nonce and the ephemeral public key are then encoded in the json with the 
encrypted value (see [ejson schema defnition](https://shopify.github.io/ejson/ejson.5.html)).  This way, the app's private key is all 
that's missing for decryption.

### Encryption Library

Encryption is hard.  Like any good developer, I tried to use a library that does it for me.  But as they say... 
the juice wasn't worth the squeeze.  I tried using [libsodium-core](https://github.com/tabrath/libsodium-core/),
which is a .NET Core version of [libsodium-net](https://github.com/adamcaudill/libsodium-net), which  is a 
wrapper around [libsodium](https://github.com/jedisct1/libsodium) for .NET, which is a portable, cross-compilable, 
installable, packageable fork of Daniel J. Bernstein's [NaCl](http://nacl.cr.yp.to/) (Networking and Cryptography library).

__HOWEVER__, this all boiled down to needing the C `libsodium` dll (or equivalent) for your platform available to the 
runtime, and doesn't really fit the .NET Core lifestyle.  I experimented with [BouncyCastle](http://www.bouncycastle.org/csharp/) ([github](https://github.com/bcgit/bc-csharp)) 
but my brain started to hurt trying to connect the dots of how they abstracted the NaCl functionality into their
library.  I pulled down [nacl.net](https://github.com/somdoron/NaCl.net) and built it for .NET Core, but it wasn't my 
project and I was hoping to find something a little less bespoke.  I even tried to rewrite 
[the go implementations](https://go.googlesource.com/crypto/+/master/salsa20/salsa20.go) into C#, but that got tedious. 
I eventually found some widely-used C# [`TweetNaCl`](https://tweetnacl.cr.yp.to/) 
code that was pasted [all over github](https://github.com/search?q=tweetnacl+extension%3Acs&type=Code), and I brought 
it into the repo ([I wish I knew who to give credit to](src/jaytwo.ejson/Internal/Crypto/readme.md)).

Bingo-bango, now I'm handling PublicBox encryption 100% managed with zero package dependencies.

---

Made with &hearts; by Jake
