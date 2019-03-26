TIMESTAMP?=$(shell date +'%Y%m%d%H%M%S')

default: clean build

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
	dotnet build ./examples/jaytwo.ejson.example.AspNetCore2_1
	dotnet build ./examples/jaytwo.ejson.example.AspNetCore1_1
	dotnet build ./examples/jaytwo.ejson.example.AspNet4_6

run:
	dotnet run --project ./src/jaytwo.ejson.CommandLine -- --help

unit-test:
	rm -rf out/testResults
	dotnet test ./test/jaytwo.ejson.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.ejson.Tests.trx"
	dotnet test ./test/jaytwo.ejson.CommandLine.Tests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.ejson.CommandLine.Tests.trx"

test: unit-test
    
pack:
	rm -rf out/packed
	cd ./src/jaytwo.ejson; \
		dotnet pack -o ../../out/packed ${PACK_ARG}
	cd ./src/jaytwo.ejson.CommandLine; \
		dotnet pack -o ../../out/packed ${PACK_ARG}
	cd ./src/jaytwo.ejson.Configuration; \
		dotnet pack -o ../../out/packed ${PACK_ARG}

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

publish:
	rm -rf out/published
	cd ./src/jaytwo.ejson.CommandLine; \
		dotnet publish -o ../../out/published

docker-test: clean
