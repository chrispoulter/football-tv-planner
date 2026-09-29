# The runtime needs the ASP.NET Core shared framework because ServiceDefaults references it.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FootballTvPlanner.FixtureSync/FootballTvPlanner.FixtureSync.csproj", "FootballTvPlanner.FixtureSync/"]
COPY ["FootballTvPlanner.ServiceDefaults/FootballTvPlanner.ServiceDefaults.csproj", "FootballTvPlanner.ServiceDefaults/"]
RUN dotnet restore "./FootballTvPlanner.FixtureSync/FootballTvPlanner.FixtureSync.csproj"
COPY . .
WORKDIR "/src/FootballTvPlanner.FixtureSync"
RUN dotnet build "./FootballTvPlanner.FixtureSync.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG GIT_COMMIT_SHA
RUN dotnet publish "./FootballTvPlanner.FixtureSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:SourceRevisionId=$GIT_COMMIT_SHA

# Runs a single sync and exits; schedule it externally (cron, Container Apps Job, etc.).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FootballTvPlanner.FixtureSync.dll"]
