FROM microsoft/dotnet:2.2.104-sdk-stretch AS dotnet-sdk
FROM microsoft/dotnet:2.2.2-runtime-alpine AS dotnet-runtime

FROM dotnet-sdk AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
    libsodium-dev \
  && rm -rf /var/lib/apt/lists/*


FROM base AS restored
WORKDIR /src
COPY jaytwo.ejson.sln .
COPY src/jaytwo.ejson/jaytwo.ejson.csproj src/jaytwo.ejson/jaytwo.ejson.csproj
COPY src/jaytwo.ejson.CommandLine/jaytwo.ejson.CommandLine.csproj src/jaytwo.ejson.CommandLine/jaytwo.ejson.CommandLine.csproj
COPY src/jaytwo.ejson.AspNetCore.Configuration/jaytwo.ejson.AspNetCore.Configuration.csproj src/jaytwo.ejson.AspNetCore.Configuration/jaytwo.ejson.AspNetCore.Configuration.csproj
COPY test/jaytwo.ejson.Tests/jaytwo.ejson.Tests.csproj test/jaytwo.ejson.Tests/jaytwo.ejson.Tests.csproj
COPY test/jaytwo.ejson.CommandLine.Tests/jaytwo.ejson.CommandLine.Tests.csproj test/jaytwo.ejson.CommandLine.Tests/jaytwo.ejson.CommandLine.Tests.csproj
COPY examples/jaytwo.ejson.example.AspNetCore2_1/jaytwo.ejson.example.AspNetCore2_1.csproj examples/jaytwo.ejson.example.AspNetCore2_1/jaytwo.ejson.example.AspNetCore2_1.csproj
RUN dotnet restore . --verbosity minimal


FROM restored AS builder
WORKDIR /src
COPY . /src
RUN make clean build


FROM builder AS publisher
WORKDIR /src
RUN make publish


FROM base AS app
WORKDIR /app
COPY --from=publisher /src/out/published /app
ENTRYPOINT ["dotnet", "jaytwo.ejson.CommandLine.dll"]
