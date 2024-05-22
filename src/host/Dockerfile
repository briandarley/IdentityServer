#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["host/Host.csproj", "host/"]
COPY ["EntityFramework.Storage/IdentityServer4.EntityFramework.Storage.csproj", "EntityFramework.Storage/"]
COPY ["Storage/IdentityServer4.Storage.csproj", "Storage/"]
COPY ["UNC.IdentityModel/UNC.IdentityModel.csproj", "UNC.IdentityModel/"]
COPY ["EntityFramework/IdentityServer4.EntityFramework.csproj", "EntityFramework/"]
COPY ["IdentityServer4/IdentityServer4.csproj", "IdentityServer4/"]
COPY ["IdentityServer4.AspNetIdentity/IdentityServer4.AspNetIdentity.csproj", "IdentityServer4.AspNetIdentity/"]
RUN dotnet restore "host/Host.csproj"
COPY . .
WORKDIR "/src/host"
RUN dotnet build "Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.dll"]