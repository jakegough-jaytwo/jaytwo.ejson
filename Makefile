BUILD_SLN=./jaytwo.ejson.sln
BUILD_DIRS=./src/jaytwo.ejson:./src/jaytwo.ejson.GlobalTool:./src/jaytwo.ejson.Configuration
BUILD_TEST_DIRS=./test/jaytwo.ejson.Tests:./test/jaytwo.ejson.GlobalTool.Tests:./test/jaytwo.ejson.example.AspNetCore3_0.IngegrationTests:./test/jaytwo.ejson.example.AspNetCore6_0.IngegrationTests:./test/jaytwo.ejson.example.AspNetCore8_0.IngegrationTests
ENABLE_COMPOSE_NETWORK=false

NUGET_SOURCE_URL?=https://api.nuget.org/v3/index.json
NUGET_API_KEY?=__missing_api_key__

TOPDIR=${CURDIR}
BUILD_TEST_RESULTS_DIR=${TOPDIR}/out/testResults
BUILD_TEST_COVERAGE_DIR=${TOPDIR}/out/coverage
BUILD_PACKED_DIR=${TOPDIR}/out/packed

DOCKER_TAG?=$(call getDockerTag,$(BUILD_SLN))
DOCKER_BASE_TAG?=${DOCKER_TAG}__base
DOCKER_BUILDER_TAG?=${DOCKER_TAG}__builder
DOCKER_BUILDER_CONTAINER?=${DOCKER_BUILDER_TAG}
DOCKER_RUN_MAKE_TARGETS?=run
TESTERNET_COMPOSE_PROJECT=${DOCKER_TAG}__testernet
TESTERNET_COMPOSE_NETWORK=${TESTERNET_COMPOSE_PROJECT}_default
TIMESTAMP?=$(call getTimestamp)

default: clean deps build test pack-beta nuget-check

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

unit-test:
	rm -rf "${BUILD_TEST_RESULTS_DIR}"
	rm -rf "${BUILD_TEST_COVERAGE_DIR}"
	for dir in $$(echo "${BUILD_TEST_DIRS}" | tr ':' '\n'); do \
		[ -n "$$dir" ] \
			&& cd "${TOPDIR}" \
			&& cd "$$dir" \
			&& dotnet test \
				--results-directory "${BUILD_TEST_RESULTS_DIR}" \
				--logger "trx;LogFileName=$$(basename $$dir).trx"; \
	done
	reportgenerator \
		"-reports:${BUILD_TEST_COVERAGE_DIR}/**/coverage.cobertura.xml" \
		"-targetdir:${BUILD_TEST_COVERAGE_DIR}/" \
		"-reportTypes:Cobertura"
	reportgenerator \
		"-reports:${BUILD_TEST_COVERAGE_DIR}/**/coverage.cobertura.xml" \
		"-targetdir:${BUILD_TEST_COVERAGE_DIR}/html" \
		"-reportTypes:Html"

pack:
	rm -rf "${BUILD_PACKED_DIR}";
	for dir in $$(echo "${BUILD_DIRS}" | tr ':' '\n'); do \
		[ -n "$$dir" ] \
			&& cd "${TOPDIR}" \
			&& cd "$$dir" \
			&& dotnet pack -o "${BUILD_PACKED_DIR}" ${PACK_ARG}; \
	done

pack-beta: PACK_ARG=--version-suffix beta-${TIMESTAMP}
pack-beta: pack

nuget-check:
	PACKED_NUPKG_FILES="$(call getNupkgFiles)"; \
	if [ -z "$$PACKED_NUPKG_FILES" ]; then \
		echo "No packages found to check." >&2; exit 1; \
	fi; \
	for nupkg in $$PACKED_NUPKG_FILES; do \
		if [ -n "$$nupkg" ]; then \
			nugetcheck \
				"$$nupkg" \
				-gte "$$nupkg" \
				--same-major \
				--fail-on-match \
			&& echo "NuGetCheck OK: $$(basename $$nupkg)" \
			|| { echo "NuGetCheck FAILED: $$(basename $$nupkg)" >&2; exit 1; }; \
		fi; \
	done

nuget-push: nuget-check
nuget-push:
	PACKED_NUPKG_FILES="$(call getNupkgFiles)"; \
	for nupkg in $$PACKED_NUPKG_FILES; do \
		if [ -n "$$nupkg" ]; then \
			dotnet nuget push \
				"$$nupkg" \
				--source "${NUGET_SOURCE_URL}" \
				--api-key "$$NUGET_API_KEY"; \
		fi; \
	done

localdev:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker compose --profile "localdev" up -d --wait --remove-orphans --build; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

localdev-logs:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker compose --profile "localdev" logs -f --tail=100; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

localdev-clean:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker compose --profile "localdev" down -v --remove-orphans; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

testernet-up:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker compose --project-name "${TESTERNET_COMPOSE_PROJECT}" --profile "testernet" up -d --wait --remove-orphans; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

testernet-run:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker run -it --rm --network "${TESTERNET_COMPOSE_NETWORK}" -e TEST_ENV=testernet ${DOCKER_BUILDER_TAG}; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

testernet-clean:
	@if [ "$(ENABLE_COMPOSE_NETWORK)" = "true" ]; then \
		docker compose --project-name "${TESTERNET_COMPOSE_PROJECT}" --profile "testernet" down -v --remove-orphans; \
	else \
		@echo "localdev is disabled. Set ENABLE_COMPOSE_NETWORK=true to enable."; \
	fi

testernet-down: testernet-clean

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

docker-pack-beta: DOCKER_RUN_MAKE_TARGETS=pack-beta
docker-pack-beta: docker-run

docker-clean:
	docker compose --profile "*" down -v --remove-orphans && echo "Compose volumes and orphans removed" || echo  "No compose project here (or nothing to clean). Skipping."
	docker rm ${DOCKER_BUILDER_CONTAINER} && echo "Container removed: ${DOCKER_BUILDER_CONTAINER}" || echo  "Nothing to clean up for: ${DOCKER_BUILDER_CONTAINER}. Skipping."
	# not removing image DOCKER_BASE_TAG since we want the layer cache to stick around (hopefully they will be cleaned up on the scheduled job)
	docker rmi ${DOCKER_BUILDER_TAG} && echo "Image removed: ${DOCKER_BUILDER_TAG}" || echo "Nothing to clean up for: ${DOCKER_BUILDER_TAG}. Skipping."
	docker rmi ${DOCKER_TAG} && echo "Image removed: ${DOCKER_TAG}" || echo "Nothing to clean up for: ${DOCKER_TAG}. Skipping."

define getDockerTag
$(shell echo '$(basename $(1))' | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/_/g' | sed 's/^_*//')
endef

define getTimestamp
$(shell date +'%Y%m%d%H%M%S')
endef

define getNupkgFiles
$(shell ls -1 ${BUILD_PACKED_DIR}/*.nupkg)
endef
