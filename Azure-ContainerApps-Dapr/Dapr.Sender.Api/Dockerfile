#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Dapr.Sender.Api/Dapr.Sender.Api.csproj", "Dapr.Sender.Api/"]
RUN dotnet restore "Dapr.Sender.Api/Dapr.Sender.Api.csproj"
COPY . .
WORKDIR "/src/Dapr.Sender.Api"
RUN dotnet build "Dapr.Sender.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dapr.Sender.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dapr.Sender.Api.dll"]