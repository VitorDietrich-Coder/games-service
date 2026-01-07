# =========================
# Base runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar csproj (cache de restore)
COPY Games.Microservice.API/Games.Microservice.API.csproj Games.Microservice.API/
COPY Games.Microservice.Application/Games.Microservice.Application.csproj Games.Microservice.Application/
COPY Games.Microservice.Domain/Games.Microservice.Domain.csproj Games.Microservice.Domain/
COPY Games.Microservice.Infrastructure/Games.Microservice.Infrastructure.csproj Games.Microservice.Infrastructure/
COPY Games.Microservice.shared/Games.Microservice.shared.csproj Games.Microservice.shared/
COPY Games.Microservice.Contracts/Games.Microservice.Contracts.csproj Games.Microservice.Contracts/

RUN dotnet restore Games.Microservice.API/Games.Microservice.API.csproj

# Copiar todo o código
COPY . .

# Build
WORKDIR /src/Games.Microservice.API
RUN dotnet build Games.Microservice.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish
RUN dotnet publish Games.Microservice.API.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Migrator (opcional)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS migrator
WORKDIR /src

COPY . .
WORKDIR /src/Games.Microservice.API

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build"]

# =========================
# Runtime final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Games.Microservice.API.dll"]
