# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/gateways/FrontendGateway/FrontendGateway.csproj", "gateways/FrontendGateway/"]
COPY src/middlewares middlewares/
RUN dotnet restore "gateways/FrontendGateway/FrontendGateway.csproj"

WORKDIR "/src/gateways/FrontendGateway"
COPY src/gateways/FrontendGateway .
RUN dotnet build "FrontendGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FrontendGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontendGateway.dll"]