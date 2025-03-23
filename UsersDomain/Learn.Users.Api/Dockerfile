#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsersDomain/Learn.Users.Api/Learn.Users.Api.csproj", "UsersDomain/Learn.Users.Api/"]
COPY ["Core/Lear.Core.Api/Learn.Core.Api.csproj", "Core/Lear.Core.Api/"]
COPY ["Core/Learn.Core.Shared/Learn.Core.Shared.csproj", "Core/Learn.Core.Shared/"]
COPY ["UsersDomain/Learn.Users.Repostory.MongoDb/Learn.Users.Repository.MongoDb.csproj", "UsersDomain/Learn.Users.Repostory.MongoDb/"]
COPY ["UsersDomain/Learn.Users.Repository/Learn.Users.Repository.csproj", "UsersDomain/Learn.Users.Repository/"]
COPY ["UsersDomain/Learn.Users.Services/Learn.Users.Services.csproj", "UsersDomain/Learn.Users.Services/"]
COPY ["UsersDomain/Learn.Users.Models/Learn.Users.Models.csproj", "UsersDomain/Learn.Users.Models/"]

# Copiar o diretório de pacotes locais para dentro da imagem Docker
COPY ["LocalNuGet", "LocalNuGet"]
RUN dotnet nuget add source "/src/LocalNuGet" -n "LocalPackages"

RUN dotnet restore "./UsersDomain/Learn.Users.Api/Learn.Users.Api.csproj"
COPY . .
WORKDIR "/src/UsersDomain/Learn.Users.Api"
RUN dotnet build "./Learn.Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Learn.Users.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learn.Users.Api.dll"]