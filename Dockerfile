# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./MCGalaxy/MCGalaxy_core.csproj /MCGalaxy/MCGalaxy_core.csproj
COPY ./CLI/MCGalaxyCLI_core.csproj MCGalaxyCLI_core.csproj

RUN dotnet restore

# Copy everything else and build
COPY ./MCGalaxy /MCGalaxy 
COPY ./CLI/Program.cs /app

RUN ls -la

RUN dotnet build -c Release
RUN dotnet publish --no-build -c Release -o out 

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1 AS local-run-env
WORKDIR /app

RUN apt-get -y update
RUN apt-get -y upgrade
RUN apt-get install -y sqlite3 libsqlite3-dev

COPY --from=build-env /app/out /app

ENTRYPOINT ["dotnet", "MCGalaxyCLI_core.dll"]

EXPOSE 25565