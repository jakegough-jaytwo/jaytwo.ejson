TIMESTAMP?=$(shell date +'%Y%m%d%H%M%S')

default: build

clean: 
	find . -name bin | xargs rm -vrf
	find . -name obj | xargs rm -vrf
	find . -name publish | xargs rm -vrf
	find . -name project.lock.json | xargs rm -vrf
	find . -name out | xargs rm -vrf

restore:
	dotnet restore . --verbosity minimal

build: restore
	dotnet build ./src/jaytwo.ejson
	dotnet build ./src/jaytwo.ejson.CommandLine
	dotnet build ./src/jaytwo.ejson.Configuration
	dotnet build ./test/jaytwo.ejson.Tests

run: build
	dotnet run --project ./src/jaytwo.ejson.CommandLine
  
try-keygen:
	dotnet run --project ./src/jaytwo.ejson.CommandLine keygen

try-encrypt:
	dotnet run --project ./src/jaytwo.ejson.CommandLine encrypt
  
try-decrypt:
	dotnet run --project ./src/jaytwo.ejson.CommandLine decrypt
  
unit-test: build
	rm -rf out/testResults
	dotnet test ./test/jaytwo.ejson.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.ejson.Tests.trx"
	dotnet test ./test/jaytwo.ejson.CommandLine.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.ejson.CommandLine.Tests.trx"

pack: build
	rm -rf out/packed
	cd ./src/jaytwo.ejson; \
		dotnet pack -o ../../out/packed ${PACK_ARG}

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

publish: build
	rm -rf out/published
	cd ./src/jaytwo.ejson; \
		dotnet publish -o ../../out/published

test: unit-test
