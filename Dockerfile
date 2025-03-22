FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-sdk
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS dotnet-runtime

FROM dotnet-sdk AS base

# For compatibility with global tools
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
  && apt-get clean \
  && apt-get autoremove\
  && rm -rf /var/lib/apt/lists/*


FROM base AS builder
WORKDIR /build
COPY . /build
RUN make deps restore


FROM builder AS publisher
RUN make publish


FROM dotnet-runtime AS app
WORKDIR /app
COPY --from=publisher /build/out/published /app
ENTRYPOINT ["dotnet", "jaytwo.ejson.CommandLine.dll"]
