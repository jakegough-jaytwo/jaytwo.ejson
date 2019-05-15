FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS dotnet-sdk
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine AS dotnet-runtime

FROM dotnet-sdk AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
    mono-devel \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*
ENV FrameworkPathOverride /usr/lib/mono/4.5/
ENV PATH="${PATH}:/root/.dotnet/tools"


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
