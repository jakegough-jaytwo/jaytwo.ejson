
### help output of the real ejson

```
$ docker run -it --rm scottatron/ejson --help
NAME:
   ejson - manage encrypted secrets using public key encryption

USAGE:
   ejson [global options] command [command options] [arguments...]

VERSION:
   1.0.0

AUTHOR:
  Burke Libbey - <burke.libbey@shopify.com>

COMMANDS:
   encrypt, e   (re-)encrypt one or more EJSON files
   decrypt, d   decrypt an EJSON file
   keygen, g    generate a new EJSON keypair
   help, h      Shows a list of commands or help for one command

GLOBAL OPTIONS:
   --keydir, -k '/opt/ejson/keys'       Directory containing EJSON keys [$EJSON_KEYDIR]
   --help, -h                           show help
   --version, -v                        print the version
```

```
$ docker run -it --rm scottatron/ejson encrypt --help
NAME:
   encrypt - (re-)encrypt one or more EJSON files

USAGE:
   command encrypt [arguments...]
```

```
$ docker run -it --rm scottatron/ejson decrypt --help
NAME:
   decrypt - decrypt an EJSON file

USAGE:
   command decrypt [command options] [arguments...]

OPTIONS:
   -o   print output to the provided file, rather than stdout
```

```
$ docker run -it --rm scottatron/ejson keygen --help
NAME:
   keygen - generate a new EJSON keypair

USAGE:
   command keygen [command options] [arguments...]

OPTIONS:
   --write, -w  rather than printing both keys, print the public and write the private into the keydir

```

```
$ docker run -it --rm scottatron/ejson keygen
Public Key:
702f26e02e168e21aaf095c4e61d6880bf3e176875c0a95be6b294058ba46249
Private Key:
f4d23c31a6b742d4616658360459b35043a02f719c6ebfce40fa9700f7206cf1
```