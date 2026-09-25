# The runtime needs the ASP.NET Core shared framework because ServiceDefaults references it.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SoccerTv.FixtureSync/SoccerTv.FixtureSync.csproj", "SoccerTv.FixtureSync/"]
COPY ["SoccerTv.ServiceDefaults/SoccerTv.ServiceDefaults.csproj", "SoccerTv.ServiceDefaults/"]
RUN dotnet restore "./SoccerTv.FixtureSync/SoccerTv.FixtureSync.csproj"
COPY . .
WORKDIR "/src/SoccerTv.FixtureSync"
RUN dotnet build "./SoccerTv.FixtureSync.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SoccerTv.FixtureSync.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runs a single sync and exits; schedule it externally (cron, Container Apps Job, etc.).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoccerTv.FixtureSync.dll"]
