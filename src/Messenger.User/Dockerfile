FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props global.json ./
COPY src/Messenger.Contracts/Messenger.Contracts.csproj src/Messenger.Contracts/
COPY src/Messenger.Messaging/Messenger.Messaging.csproj src/Messenger.Messaging/
COPY src/Messenger.Shared/Messenger.Shared.csproj src/Messenger.Shared/
COPY src/Messenger.User/Messenger.User.csproj src/Messenger.User/

RUN dotnet restore src/Messenger.User/Messenger.User.csproj

COPY src/ src/

RUN dotnet publish src/Messenger.User/Messenger.User.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build --chown=app:app /app/publish .

USER app

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Messenger.User.dll"]
