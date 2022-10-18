#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Net.ServiceBus.SingleJob/Net.ServiceBus.SingleJob.csproj", "Net.ServiceBus.SingleJob/"]
RUN dotnet restore "Net.ServiceBus.SingleJob/Net.ServiceBus.SingleJob.csproj"
COPY . .
WORKDIR "/src/Net.ServiceBus.SingleJob"
RUN dotnet build "Net.ServiceBus.SingleJob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Net.ServiceBus.SingleJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Net.ServiceBus.SingleJob.dll"]