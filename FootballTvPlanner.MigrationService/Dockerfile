# The runtime needs the ASP.NET Core shared framework because ServiceDefaults references it.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FootballTvPlanner.MigrationService/FootballTvPlanner.MigrationService.csproj", "FootballTvPlanner.MigrationService/"]
COPY ["FootballTvPlanner.Data/FootballTvPlanner.Data.csproj", "FootballTvPlanner.Data/"]
COPY ["FootballTvPlanner.ServiceDefaults/FootballTvPlanner.ServiceDefaults.csproj", "FootballTvPlanner.ServiceDefaults/"]
RUN dotnet restore "./FootballTvPlanner.MigrationService/FootballTvPlanner.MigrationService.csproj"
COPY . .
WORKDIR "/src/FootballTvPlanner.MigrationService"
RUN dotnet build "./FootballTvPlanner.MigrationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG GIT_COMMIT_SHA
RUN dotnet publish "./FootballTvPlanner.MigrationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish -r linux-x64 --self-contained false /p:PublishReadyToRun=true /p:UseAppHost=false /p:SourceRevisionId=$GIT_COMMIT_SHA

# Applies pending migrations and exits; run it before deploying a new API version.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FootballTvPlanner.MigrationService.dll"]
