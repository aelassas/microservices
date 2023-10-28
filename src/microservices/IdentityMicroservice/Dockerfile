# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/microservices/IdentityMicroservice/IdentityMicroservice.csproj", "microservices/IdentityMicroservice/"]
COPY src/middlewares middlewares/
RUN dotnet restore "microservices/IdentityMicroservice/IdentityMicroservice.csproj"

WORKDIR "/src/microservices/IdentityMicroservice"
COPY src/microservices/IdentityMicroservice .
RUN dotnet build "IdentityMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityMicroservice.dll"]