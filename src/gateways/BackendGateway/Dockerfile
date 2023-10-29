# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/gateways/BackendGateway/BackendGateway.csproj", "gateways/BackendGateway/"]
COPY src/middlewares middlewares/
RUN dotnet restore "gateways/BackendGateway/BackendGateway.csproj"

WORKDIR "/src/gateways/BackendGateway"
COPY src/gateways/BackendGateway .
RUN dotnet build "BackendGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendGateway.dll"]