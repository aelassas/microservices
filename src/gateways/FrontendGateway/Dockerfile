# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontendGateway.dll"]