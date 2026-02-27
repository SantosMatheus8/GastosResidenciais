# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache icu-libs icu-data-full
EXPOSE 5285
ENV ASPNETCORE_URLS=http://+:5285/
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Etapa de build (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ./ResiGa.Bkd.sln .
COPY ./ResiGa.Bkd.Api ./ResiGa.Bkd.Api
COPY ./ResiGa.Bkd.Domain ./ResiGa.Bkd.Domain
COPY ./ResiGa.Bkd.Infra ./ResiGa.Bkd.Infra
COPY ./ResiGa.Bkd.IoC ./ResiGa.Bkd.IoC
COPY ./ResiGa.Bkd.Service ./ResiGa.Bkd.Service

WORKDIR /src/ResiGa.Bkd.Api
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ResiGa.Bkd.Api.dll"]
