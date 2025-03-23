#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["QuizzDomain/Learn.Quizz.Api/Learn.Quizz.Api.csproj", "QuizzDomain/Learn.Quizz.Api/"]
COPY ["AIDomain/Learn.AI.Client/Learn.AI.Client.csproj", "AIDomain/Learn.AI.Client/"]
COPY ["Core/Learn.Core.Shared/Learn.Core.Shared.csproj", "Core/Learn.Core.Shared/"]
COPY ["AIDomain/Learn.AI.Models/Learn.AI.Models.csproj", "AIDomain/Learn.AI.Models/"]
COPY ["Core/Lear.Core.Api/Learn.Core.Api.csproj", "Core/Lear.Core.Api/"]
COPY ["QuizzDomain/Learn.Quizz.Models/Learn.Quizz.Models.csproj", "QuizzDomain/Learn.Quizz.Models/"]
COPY ["QuizzDomain/Learn.Quizz.Repository.MongoDb/Learn.Quizz.Repository.MongoDb.csproj", "QuizzDomain/Learn.Quizz.Repository.MongoDb/"]
COPY ["QuizzDomain/Learn.Quizz.Repository/Learn.Quizz.Repository.csproj", "QuizzDomain/Learn.Quizz.Repository/"]
COPY ["QuizzDomain/Learn.Quizz.Services/Learn.Quizz.Services.csproj", "QuizzDomain/Learn.Quizz.Services/"]
RUN dotnet restore "./QuizzDomain/Learn.Quizz.Api/Learn.Quizz.Api.csproj"
COPY . .
WORKDIR "/src/QuizzDomain/Learn.Quizz.Api"
RUN dotnet build "./Learn.Quizz.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Learn.Quizz.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learn.Quizz.Api.dll"]