FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Rest.API/.", "Rest.API/."]
COPY ["Infrastructure/.", "Infrastructure/."]
# RUN ls -R
RUN dotnet build "Rest.API/Rest.API.csproj" -c Release -o /app/build

FROM build AS publish
# RUN ls -R
RUN dotnet publish "Rest.API/Rest.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# RUN ls -R
ENTRYPOINT ["dotnet", "Rest.API.dll"]