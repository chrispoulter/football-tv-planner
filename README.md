# Soccer TV

Lists the football shown on UK TV and streaming services, day by day. Signed-in users can star games to build a personal schedule. They can then add games to Google Calendar or Outlook, or subscribe to a private calendar feed that stays in sync with their schedule.

The app is built on the [halcyon-dotnet](../halcyon-dotnet) template: a .NET 10 minimal API, EF Core with PostgreSQL, React 19 with Vite, TanStack Query and shadcn/ui, and Aspire.

## Features

- **Fixtures by day.** Pick a day from the 14-day strip. Filter by competition or by broadcaster (Sky, TNT, Amazon, BBC, Premier Sports…). Kick-off times are shown in UK time.
- **My Schedule.** Star a game to add it to your schedule.
- **Add to calendar.** Each game has one-click links for Google Calendar, Outlook.com and Outlook (Microsoft 365), plus a `.ics` download.
- **Calendar subscription.** Each user gets a private `webcal://` feed of their starred games. The link can be reset, which stops the old one working.
- **Reminders.** Reminders are calendar alarms (`VALARM`), not server-sent notifications. You choose how long before kick-off under **My Account → Update Profile**.

## Getting Started

### Prerequisites

- .NET SDK (see `global.json`)
- Node.js 24
- Docker, which Aspire uses to run PostgreSQL and Mailpit

### Run the application

```
dotnet run --project "SoccerTv.AppHost/SoccerTv.AppHost.csproj"
```

Aspire starts PostgreSQL, Mailpit, the API and the web app. The web app runs at http://localhost:5173, and the API docs (Scalar) are available from the Aspire dashboard.

On startup, the API applies migrations, seeds the admin user from the `Seed` settings, and syncs fixtures. Syncing then repeats every `FixtureProvider:SyncIntervalHours`.

> The AppHost uses fixed host ports for PostgreSQL (5432) and Mailpit (1025/8025). Stop any other containers using those ports first, or change the ports in `SoccerTv.AppHost/AppHost.cs`.

### Configuration

Create `SoccerTv.Api/appsettings.Development.json` to override `appsettings.json` locally. Git ignores this file. The settings specific to this app are:

```json
{
  "FixtureProvider": {
    "Provider": "Mock",
    "SyncIntervalHours": 6,
    "DaysAhead": 14
  }
}
```

## Fixture data

Fixtures come in through `IFixtureProvider` (`SoccerTv.Api/Common/Fixtures`). `FixtureSyncService` calls the configured provider on a timer. It upserts competitions, teams and channels by name, and fixtures by `(Source, ExternalId)`. It also replaces each fixture's broadcasters on every sync, so the rest of the app never talks to a provider directly.

The only provider for now is `MockFixtureProvider`. It generates deterministic fixtures with kick-off slots and channels based on the current UK rights holders, and no Saturday 15:00 games because of the UK TV blackout.

### Adding a real provider

1. Implement `IFixtureProvider` with a unique `Source` and stable external IDs.
2. Register it in `FixtureSyncExtensions.AddFixtureSync` under a new `FixtureProvider:Provider` name.

Options considered:

| Source | Pros | Cons |
| --- | --- | --- |
| [TheSportsDB](https://www.thesportsdb.com/documentation) premium (~$9/month) | The v2 API can filter TV events by country and day (`/filter/tv/country/united_kingdom`) and returns channel names | Paid; data is community-maintained |
| [football-data.org](https://www.football-data.org/) (free tier) + manual channels | Reliable fixtures | No broadcaster data, so channels need an admin screen |
| Scraping a listings site (e.g. live-footballontv.com) | Very complete UK listings | Fragile, and likely against the site's terms |

Official fixture lists for the English and Scottish leagues are licensed through Football DataCo. Only personal, non-commercial use has been considered here.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
