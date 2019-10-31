FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS dotnet-sdk
FROM mcr.microsoft.com/dotnet/core/runtime:3.0-alpine AS dotnet-runtime

FROM dotnet-sdk AS base

# Install past versions of .Net Core
RUN wget -q https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
  && dpkg -i packages-microsoft-prod.deb \
  && apt-get update \
  && apt-get install -y --no-install-recommends \
    dotnet-sdk-2.1 \
    dotnet-sdk-2.2 \
  && apt-get clean \
  && apt-get autoremove \
  && rm -rf /var/lib/apt/lists/* \
  && rm -rf packages-microsoft-prod.deb

# TODO: figure out how to install .net core 1.1 for the tests
#       https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-1.1.14-linux-ubuntu-16.04-x64-binaries

# For compatibility with global tools
ENV PATH="${PATH}:/root/.dotnet/tools"

# Install Mono (for Framework build target)
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
    mono-devel \
  && apt-get clean \
  && apt-get autoremove\
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
