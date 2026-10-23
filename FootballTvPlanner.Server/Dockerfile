# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the web frontend served from wwwroot
FROM node:24-alpine AS web
WORKDIR /web
COPY ["FootballTvPlanner.Client/package.json", "FootballTvPlanner.Client/package-lock.json", "./"]
RUN npm ci
COPY FootballTvPlanner.Client/ .
ARG GIT_COMMIT_SHA
ENV GIT_COMMIT_SHA=$GIT_COMMIT_SHA
RUN npm run build


# This stage restores packages and copies the source for the publish stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FootballTvPlanner.Server/FootballTvPlanner.Server.csproj", "FootballTvPlanner.Server/"]
COPY ["FootballTvPlanner.Data/FootballTvPlanner.Data.csproj", "FootballTvPlanner.Data/"]
COPY ["FootballTvPlanner.ServiceDefaults/FootballTvPlanner.ServiceDefaults.csproj", "FootballTvPlanner.ServiceDefaults/"]
RUN dotnet restore "./FootballTvPlanner.Server/FootballTvPlanner.Server.csproj" -r linux-x64 /p:PublishReadyToRun=true
COPY . .
WORKDIR "/src/FootballTvPlanner.Server"

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG GIT_COMMIT_SHA
RUN dotnet publish "./FootballTvPlanner.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish -r linux-x64 --no-restore --self-contained false /p:PublishReadyToRun=true /p:UseAppHost=false /p:SourceRevisionId=$GIT_COMMIT_SHA

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=web /web/dist ./wwwroot
ENTRYPOINT ["dotnet", "FootballTvPlanner.Server.dll"]