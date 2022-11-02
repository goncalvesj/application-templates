#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Net.ServiceBus.Worker/Net.ServiceBus.Worker.csproj", "Net.ServiceBus.Worker/"]
RUN dotnet restore "Net.ServiceBus.Worker/Net.ServiceBus.Worker.csproj"
COPY . .
WORKDIR "/src/Net.ServiceBus.Worker"
RUN dotnet build "Net.ServiceBus.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Net.ServiceBus.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Net.ServiceBus.Worker.dll"]