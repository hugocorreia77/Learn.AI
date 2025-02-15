#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AIDomain/Learn.AI.Api/Learn.AI.Api.csproj", "AIDomain/Learn.AI.Api/"]
COPY ["Core/Lear.Core.Api/Learn.Core.Api.csproj", "Core/Lear.Core.Api/"]
COPY ["AIDomain/Learn.AI.Repository.MongoDb/Learn.AI.Repository.MongoDb.csproj", "AIDomain/Learn.AI.Repository.MongoDb/"]
COPY ["AIDomain/Learn.AI.Repository/Learn.AI.Repository.csproj", "AIDomain/Learn.AI.Repository/"]
COPY ["Core/Learn.Core.Shared/Learn.Core.Shared.csproj", "Core/Learn.Core.Shared/"]
COPY ["AIDomain/Learn.AI.Services/Learn.AI.Services.csproj", "AIDomain/Learn.AI.Services/"]
COPY ["AIDomain/Learn.AI.Models/Learn.AI.Models.csproj", "AIDomain/Learn.AI.Models/"]
RUN dotnet restore "./AIDomain/Learn.AI.Api/Learn.AI.Api.csproj"
COPY . .
WORKDIR "/src/AIDomain/Learn.AI.Api"
RUN dotnet build "./Learn.AI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Learn.AI.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learn.AI.Api.dll"]