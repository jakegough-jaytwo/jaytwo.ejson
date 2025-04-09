TOPDIR=$(shell pwd)

BUILD_SLN=./jaytwo.ejson.sln
BUILD_DIRS=./src/jaytwo.ejson:./src/jaytwo.ejson.GlobalTool:./src/jaytwo.ejson.Configuration
BUILD_TEST_DIRS=./test/jaytwo.ejson.Tests:./test/jaytwo.ejson.GlobalTool.Tests:./test/jaytwo.ejson.example.AspNetCore3_0.IngegrationTests:./test/jaytwo.ejson.example.AspNetCore6_0.IngegrationTests:./test/jaytwo.ejson.example.AspNetCore8_0.IngegrationTests
BUILD_TRX_FILENAME=jaytwo.ejson.Tests.trx
BUILD_PACKED_DIR=${TOPDIR}/out/packed

NUGET_SOURCE_URL?=https://api.nuget.org/v3/index.json
NUGET_API_KEY?=__missing_api_key__

DOCKER_TAG?=$(call getDockerTag,$(BUILD_SLN))
DOCKER_BASE_TAG?=${DOCKER_TAG}__base
DOCKER_BUILDER_TAG?=${DOCKER_TAG}__builder
DOCKER_BUILDER_CONTAINER?=${DOCKER_BUILDER_TAG}
DOCKER_RUN_MAKE_TARGETS?=run
TIMESTAMP?=$(call getTimestamp)

default: clean build

deps:
	dotnet tool install -g dotnet-reportgenerator-globaltool
	dotnet tool install -g jaytwo.NuGetCheck.GlobalTool

clean:
	find . -name bin | xargs --no-run-if-empty rm -vrf
	find . -name obj | xargs --no-run-if-empty rm -vrf
	rm -rf ${TOPDIR}/out

restore:
	dotnet restore . --verbosity minimal

build: restore
	dotnet build "${BUILD_SLN}"

test: unit-test

unit-test: build
	rm -rf "./out/testResults"
	rm -rf "./out/coverage"
	for dir in $$(echo "${BUILD_TEST_DIRS}" | tr ':' '\n'); do \
		[ -n "$$dir" ] \
			&& cd "${TOPDIR}" \
			&& cd "$$dir" \
			&& dotnet test \
				--results-directory "${TOPDIR}/out/testResults" \
				--logger "trx;LogFileName=${BUILD_TRX_FILENAME}"; \
	done
	cd "${BUILD_TEST_DIR}"; \
		dotnet test \
		--results-directory "${TOPDIR}/out/testResults" \
		--logger "trx;LogFileName=${BUILD_TRX_FILENAME}"
	reportgenerator \
		"-reports:${TOPDIR}/out/coverage/**/coverage.cobertura.xml" \
		"-targetdir:${TOPDIR}/out/coverage/" \
		"-reportTypes:Cobertura"
	reportgenerator \
		"-reports:${TOPDIR}/out/coverage/**/coverage.cobertura.xml" \
		"-targetdir:${TOPDIR}/out/coverage/html" \
		"-reportTypes:Html"

pack:
	rm -rf "${BUILD_PACKED_DIR}"; \
	for dir in $$(echo "${BUILD_DIRS}" | tr ':' '\n'); do \
		[ -n "$$dir" ] \
			&& cd "${TOPDIR}" \
			&& cd "$$dir" \
			&& dotnet pack -o "${BUILD_PACKED_DIR}" ${PACK_ARG}; \
	done

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

nuget-check:
	PACKED_NUPKG_FILE="$(call getNupkg)"; \
	nugetcheck "$$PACKED_NUPKG_FILE" -gte "$$PACKED_NUPKG_FILE" --same-major --fail-on-match && echo "Ready to push!" || "Failed NuGetCheck; cannot push."

nuget-push: nuget-check
nuget-push:
	PACKED_NUPKG_FILE="$(call getNupkg)"; \
	dotnet nuget push "$$PACKED_NUPKG_FILE" --source "${NUGET_SOURCE_URL}" --api-key "$$NUGET_API_KEY"

docker-builder:
	# building the base image to force caching those layers in an otherwise discarded stage of the multistage dockerfile
	docker build -t ${DOCKER_BASE_TAG} . --target base --pull
	docker build -t ${DOCKER_BUILDER_TAG} . --target builder --pull

docker: docker-builder
	docker build -t ${DOCKER_TAG} . --pull

docker-run:
	docker run --name ${DOCKER_BUILDER_CONTAINER} ${DOCKER_BUILDER_TAG} make ${DOCKER_RUN_MAKE_TARGETS} || EXIT_CODE=$$? ; \
	docker cp ${DOCKER_BUILDER_CONTAINER}:build/out ./ || echo "Could not copy out of builder container: Container not found: ${DOCKER_BUILDER_CONTAINER}"; \
	docker rm ${DOCKER_BUILDER_CONTAINER} && echo "Container removed: ${DOCKER_BUILDER_CONTAINER}" || echo "Container not found: ${DOCKER_BUILDER_CONTAINER}"; \
	exit $$EXIT_CODE

docker-copy-from-builder-output:
	docker cp ${DOCKER_BUILDER_CONTAINER}:build/out ./ || echo "Could not copy out of builder container: Container not found: ${DOCKER_BUILDER_CONTAINER}"

docker-test: DOCKER_RUN_MAKE_TARGETS=test
docker-test: docker-run

docker-pack: DOCKER_RUN_MAKE_TARGETS=pack
docker-pack: docker-run

docker-pack: DOCKER_RUN_MAKE_TARGETS=pack-beta
docker-pack: docker-run

docker-clean:
	docker rm ${DOCKER_BUILDER_CONTAINER} && echo "Container removed: ${DOCKER_BUILDER_CONTAINER}" || echo  "Nothing to clean up for: ${DOCKER_BUILDER_CONTAINER}"
	# not removing image DOCKER_BASE_TAG since we want the layer cache to stick around (hopefully they will be cleaned up on the scheduled job)
	docker rmi ${DOCKER_BUILDER_TAG} && echo "Image removed: ${DOCKER_BUILDER_TAG}" || echo "Nothing to clean up for: ${DOCKER_BUILDER_TAG}"
	docker rmi ${DOCKER_TAG} && echo "Image removed: ${DOCKER_TAG}" || echo "Nothing to clean up for: ${DOCKER_TAG}"

define getDockerTag
$(shell echo '$(basename $(1))' | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/_/g' | sed 's/^_*//')
endef

define getTimestamp
$(shell date +'%Y%m%d%H%M%S')
endef

define getNupkg
$(shell ls -1 ${BUILD_PACKED_DIR}/*.nupkg)
endef
