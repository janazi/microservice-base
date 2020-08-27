#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 1011

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MicroserviceBase.Api/MicroserviceBase.Api.csproj", "MicroserviceBase.Api/"]
RUN dotnet restore "MicroserviceBase.Api/MicroserviceBase.Api.csproj"
COPY . .
WORKDIR "/src/MicroserviceBase.Api"
RUN dotnet build "MicroserviceBase.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroserviceBase.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceBase.Api.dll"]