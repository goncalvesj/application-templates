FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AppService-FrontDoor-PrivateEndpoints.csproj", "./"]
RUN dotnet restore "AppService-FrontDoor-PrivateEndpoints.csproj" -a "$TARGETARCH"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AppService-FrontDoor-PrivateEndpoints.csproj" -c Release -a "$TARGETARCH" -o /app/build

FROM build AS publish
RUN dotnet publish "AppService-FrontDoor-PrivateEndpoints.csproj" -c Release -a "$TARGETARCH" \ 
    -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppService-FrontDoor-PrivateEndpoints.dll"]
