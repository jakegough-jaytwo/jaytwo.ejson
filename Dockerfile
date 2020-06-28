FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS dotnet-sdk
#FROM mcr.microsoft.com/dotnet/core/runtime:3.0-alpine AS dotnet-runtime

FROM dotnet-sdk AS base

RUN apt-get update \
  && apt-get install -y --no-install-recommends \
# Install Make for makefile support
    make \
# Install Mono (for .NET Framework build target)
    mono-devel \
  && apt-get clean \
  && apt-get autoremove \
  && rm -rf /var/lib/apt/lists/*

ENV FrameworkPathOverride /usr/lib/mono/4.5/
ENV PATH="${PATH}:/root/.dotnet/tools"

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

FROM base AS builder
WORKDIR /build
COPY . /build
RUN make deps restore
