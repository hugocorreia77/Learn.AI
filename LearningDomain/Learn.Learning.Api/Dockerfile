#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LearningDomain/Learn.Learning.Api/Learn.Learning.Api.csproj", "LearningDomain/Learn.Learning.Api/"]
COPY ["AIDomain/Learn.AI.Client/Learn.AI.Client.csproj", "AIDomain/Learn.AI.Client/"]
COPY ["Core/Learn.Core.Shared/Learn.Core.Shared.csproj", "Core/Learn.Core.Shared/"]
COPY ["AIDomain/Learn.AI.Models/Learn.AI.Models.csproj", "AIDomain/Learn.AI.Models/"]
COPY ["Core/Lear.Core.Api/Learn.Core.Api.csproj", "Core/Lear.Core.Api/"]
COPY ["LearningDomain/Learn.Learning.Repository.MongoDb/Learn.Learning.Repository.MongoDb.csproj", "LearningDomain/Learn.Learning.Repository.MongoDb/"]
COPY ["LearningDomain/Learn.Learning.Repository/Learn.Learning.Repository.csproj", "LearningDomain/Learn.Learning.Repository/"]
COPY ["LearningDomain/Learn.Learning.Services/Learn.Learning.Services.csproj", "LearningDomain/Learn.Learning.Services/"]
COPY ["LearningDomain/Learn.Learning.Models/Learn.Learning.Models.csproj", "LearningDomain/Learn.Learning.Models/"]
RUN dotnet restore "./LearningDomain/Learn.Learning.Api/Learn.Learning.Api.csproj"
COPY . .
WORKDIR "/src/LearningDomain/Learn.Learning.Api"
RUN dotnet build "./Learn.Learning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Learn.Learning.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learn.Learning.Api.dll"]