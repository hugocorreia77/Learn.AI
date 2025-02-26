#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SecurityDomain/Learn.Security.Api/Learn.Security.Api.csproj", "SecurityDomain/Learn.Security.Api/"]
COPY ["Core/Lear.Core.Api/Learn.Core.Api.csproj", "Core/Lear.Core.Api/"]
COPY ["Core/Learn.Core.Shared/Learn.Core.Shared.csproj", "Core/Learn.Core.Shared/"]
COPY ["SecurityDomain/Learn.Security.Services/Learn.Security.Services.csproj", "SecurityDomain/Learn.Security.Services/"]
COPY ["UsersDomain/Learn.Users.Repostory.MongoDb/Learn.Users.Repository.MongoDb.csproj", "UsersDomain/Learn.Users.Repostory.MongoDb/"]
COPY ["UsersDomain/Learn.Users.Repository/Learn.Users.Repository.csproj", "UsersDomain/Learn.Users.Repository/"]
COPY ["SecurityDomain/Learn.Security.Models/Learn.Security.Models.csproj", "SecurityDomain/Learn.Security.Models/"]
COPY ["SecurityDomain/Learn.Security.Repository/Learn.Security.Repository.csproj", "SecurityDomain/Learn.Security.Repository/"]
RUN dotnet restore "./SecurityDomain/Learn.Security.Api/Learn.Security.Api.csproj"
COPY . .
WORKDIR "/src/SecurityDomain/Learn.Security.Api"
RUN dotnet build "./Learn.Security.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Learn.Security.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learn.Security.Api.dll"]