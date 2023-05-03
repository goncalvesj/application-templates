#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#RUN apt-get update
#RUN apt-get install -y curl
#RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
#RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
#RUN apt-get install -y nodejs
#
#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
#RUN apt-get update
#RUN apt-get install -y curl
#RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
#RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
#RUN apt-get install -y nodejs
#WORKDIR /src
#COPY ["WebApp/EventSourcing.csproj", "WebApp/"]
#COPY ["EventSourcing.Common/EventSourcing.Common.csproj", "EventSourcing.Common/"]
#RUN dotnet restore "WebApp/EventSourcing.csproj"
#COPY . .
#WORKDIR "/src/WebApp"
#RUN npm install
#RUN dotnet build "EventSourcing.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "EventSourcing.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "EventSourcing.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
COPY ./bin/Release/netcoreapp3.1/publish /app
ENTRYPOINT ["dotnet", "EventSourcing.dll"]
