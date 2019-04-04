TIMESTAMP?=$(shell date +'%Y%m%d%H%M%S')
DOCKER_TAG?=jaytwo_ejson

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
	dotnet build ./jaytwo.ejson.sln ${BUILD_ARG}

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
#	dotnet test ./test/jaytwo.ejson.example.AspNetCore1_1.IngegrationTests \
#		--results-directory ../../out/testResults \
#		--logger "trx;LogFileName=jaytwo.ejson.example.AspNetCore2_1.IngegrationTests.trx"
	dotnet test ./test/jaytwo.ejson.example.AspNetCore2_1.IngegrationTests \
		--results-directory ../../out/testResults \
		--logger "trx;LogFileName=jaytwo.ejson.example.AspNetCore1_1.IngegrationTests.trx"
    
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

DOCKER_BUILDER_TAG?=${DOCKER_TAG}__builder
DOCKER_BUILDER_CONTAINER?=${DOCKER_BUILDER_TAG}
docker-build:
	docker build -t ${DOCKER_BUILDER_TAG} . --target builder

DOCKER_RUN_MAKE_TARGETS?=pack
docker-run:
	# A; B    # Run A and then B, regardless of success of A
	# A && B  # Run B if and only if A succeeded
	# A || B  # Run B if and only if A failed
	# A &     # Run A in background.
	docker run --name ${DOCKER_BUILDER_CONTAINER} ${DOCKER_BUILDER_TAG} make ${DOCKER_RUN_MAKE_TARGETS}; \
	docker cp ${DOCKER_BUILDER_CONTAINER}:build/out ./; \
	docker rm ${DOCKER_BUILDER_CONTAINER}

docker-unit-test-only: DOCKER_RUN_MAKE_TARGETS=unit-test
docker-unit-test-only: docker-run

docker-unit-test: docker-build docker-unit-test-only

docker-pack-only: DOCKER_RUN_MAKE_TARGETS=pack
docker-pack-only: docker-run

docker-pack: docker-build docker-pack-only

docker-pack-beta-only: DOCKER_RUN_MAKE_TARGETS=pack-beta
docker-pack-beta-only: docker-run

docker-pack-beta: docker-build docker-pack-beta-only

docker-clean:
	docker rm ${DOCKER_BUILDER_CONTAINER} || echo "Container not found: ${DOCKER_BUILDER_CONTAINER}"
	docker rmi ${DOCKER_BUILDER_TAG} || echo "Image not found: ${DOCKER_BUILDER_TAG}"
