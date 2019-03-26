FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS dotnet-sdk
FROM mcr.microsoft.com/dotnet/core/runtime:2.1 AS dotnet-runtime

FROM dotnet-sdk AS base
RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    make \
  && rm -rf /var/lib/apt/lists/*


FROM base AS restored
WORKDIR /src
COPY jaytwo.ejson.sln .
COPY src/jaytwo.ejson/jaytwo.ejson.csproj src/jaytwo.ejson/jaytwo.ejson.csproj
COPY src/jaytwo.ejson.CommandLine/jaytwo.ejson.CommandLine.csproj src/jaytwo.ejson.CommandLine/jaytwo.ejson.CommandLine.csproj
COPY src/jaytwo.ejson.Configuration/jaytwo.ejson.Configuration.csproj src/jaytwo.ejson.Configuration/jaytwo.ejson.Configuration.csproj
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


FROM dotnet-runtime AS app
WORKDIR /app
COPY --from=publisher /src/out/published /app
ENTRYPOINT ["dotnet", "jaytwo.ejson.CommandLine.dll"]
