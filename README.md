# Football TV Planner

Shows the football on UK TV and streaming services, day by day. Signed-in users can star games to build their own schedule, then add games to their calendar or subscribe to a private feed that keeps up with the schedule.

## Features

- **Fixtures by day.** Pick one of the next 14 days, then filter by competition or channel. Kick-off times, and which games fall on which day, use the viewer's local time zone.
- **My Schedule.** Star a game to add it to your schedule. The **My Schedule** filter narrows the list to your starred games.
- **Add to calendar.** Each game has links for Google Calendar and Outlook.com, plus an `.ics` download. Events include a reminder 30 minutes before kick-off.
- **Calendar feed.** Each user gets a private `webcal://` feed of their starred games, with one-click subscribe for Google and Outlook. Resetting the link stops the old one from working.
- **Accounts.** Email and password registration with email confirmation and password reset, optional Google sign-in, linking Google to an existing account, two-factor authentication with recovery codes, and account deletion.

## Projects

| Project                    | Description                                                                                                                                                                   |
| -------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `FootballTvPlanner.Api`             | .NET 10 minimal API. ASP.NET Core Identity with cookie auth, EF Core with PostgreSQL, and Scalar API docs at `/`. Owns the database schema and applies migrations on startup. |
| `FootballTvPlanner.FixtureSync`     | Console job that pulls fixtures from a provider into the `fixtures` table, then exits. Run it on a schedule.                                                                  |
| `FootballTvPlanner.Web`             | React 19 app built with Vite, React Router, TanStack Query, shadcn/ui and Tailwind CSS.                                                                                       |
| `FootballTvPlanner.AppHost`         | Aspire app host for local development.                                                                                                                                        |
| `FootballTvPlanner.ServiceDefaults` | Shared Aspire setup: OpenTelemetry, health checks, service discovery and resilience.                                                                                          |

## Getting started

### Prerequisites

- .NET SDK 10 (see `global.json`)
- Node.js 24
- Docker, which Aspire uses to run PostgreSQL and Mailpit

### Run the app

```
dotnet run --project FootballTvPlanner.AppHost
```

Aspire starts PostgreSQL, Mailpit, the API, the fixture sync and the web app. When they're up:

- Web app: http://localhost:5173
- API docs (Scalar): the API's link in the Aspire dashboard
- Mailpit, which receives confirmation and password reset emails: http://localhost:8025

When the API starts, it applies migrations. The fixture sync runs once after the API starts. To pick up new fixtures, restart the `fixture-sync` resource from the dashboard.

> PostgreSQL (5432) and Mailpit (1025 and 8025) use fixed host ports and persistent containers. Stop anything else that uses those ports, or change them in `FootballTvPlanner.AppHost/AppHost.cs`.

You can also run everything with `docker compose up`, which builds the three app images and uses the same ports.

## Configuration

`appsettings.json` in each project lists every setting, with blank values where you need to supply your own. To override them locally, create an `appsettings.Development.json` next to it. Git ignores these files. You can also use user secrets (the API has these set up) or environment variables such as `Authentication__Google__ClientId`.

### Google sign-in

Google sign-in is optional. It's turned on only when `Authentication:Google:ClientId` is set.

1. In Google Cloud Console, go to **APIs & Services → Credentials** and create an OAuth client ID of type **Web application**.
2. Add these authorised redirect URIs. The web app proxies `/api` to the API, so the callback goes through the web app's origin:
   - `http://localhost:5173/api/signin-google`
   - `https://<web app host>/api/signin-google`
3. Put the credentials in `FootballTvPlanner.Api/appsettings.Development.json`:

   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "<client id>",
         "ClientSecret": "<client secret>"
       }
     }
   }
   ```

### Fixture provider

The fixture sync reads fixtures from the provider named in `FixtureSync:Provider`:

| Provider         | Description                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `Mock` (default) | Generates repeatable fake fixtures `FixtureSync:Mock:DaysAhead` days ahead. The same dates always give the same games. |

Set `FixtureSync:Provider` in `FootballTvPlanner.FixtureSync/appsettings.Development.json` to switch.

### Email

The API sends email over SMTP using the `Mail` connection string, for example `Endpoint=smtp://localhost:1025`. `Email:NoReplyAddress` is the sender. `Email:SiteUrl` is the web app URL used for links in emails. Aspire sets `Email:SiteUrl` for you.

## How it works

### Authentication and the `/api` proxy

The web app never calls the API directly. It proxies `/api/*` to the API unchanged, since every API endpoint is mapped under `/api`: Vite does this in development (`API_URL`) and nginx does it in the container (`API_URL`). This keeps the Identity cookie first-party even when the API is hosted on a different domain.

The proxy sends the public host and scheme in `X-FootballTvPlanner-*` headers. The API reads them in `ForwardedHeadersExtensions` so it can build correct external login redirect URIs. Custom header names are used so they can't clash with the `X-Forwarded-*` headers that the hosting provider sets.

### Dates and times

Every time is stored and returned in UTC: `timestamptz` in Postgres, ISO 8601 strings in the API, and UTC `DTSTART` values in `.ics` files. The web app converts them to the viewer's time zone (`src/lib/local-time.ts`). When you pick a day, the web app sends it to `GET /api/fixtures` as a UTC `from`/`to` range, so the API doesn't need to know the viewer's time zone. The only code that knows about UK time is in the fixture providers, which convert UK kick-off times to UTC with `UkTime`.

### Fixture sync

Each run, `FixtureSyncer`:

- upserts fixtures by `(Source, ExternalId)`
- replaces each fixture's channel list
- deletes upcoming fixtures from the same source that the provider no longer lists, because a moved game comes back under a new ID

If a provider returns nothing, the run fails instead of deleting every upcoming fixture.

The API owns the schema. `FootballTvPlanner.FixtureSync/Data/Fixture.cs` maps the same table, so update it whenever the API's `Fixture` entity changes.

To add a provider:

1. Implement `IFixtureProvider` with a unique `Source` and external IDs that don't change between runs.
2. Register it in `FixtureProviderExtensions.AddFixtureProvider` under a new `FixtureSync:Provider` name.

## Development

### Database migrations

Migrations are in `FootballTvPlanner.Api/Migrations`. To add one:

```
dotnet tool restore
dotnet ef migrations add <Name> --project FootballTvPlanner.Api
```

### Formatting and linting

```
dotnet csharpier format .

cd FootballTvPlanner.Web
npm run lint
npm run format
```

## Deployment

On every push to `main`, `develop`, `feature/**`, `release/**` and `hotfix/**`, GitHub Actions builds and lints everything and pushes three images to GitHub Container Registry: `-api`, `-fixture-sync` and `-web`.

In production:

- **API:** set `ConnectionStrings__Database`, `ConnectionStrings__Mail`, `Email__SiteUrl`, `Email__NoReplyAddress` and, optionally, `Authentication__Google__*`.
- **Web:** set `API_URL` to the API's URL.
- **Fixture sync:** set `ConnectionStrings__Database` and `FixtureSync__Provider`, and run it as a scheduled job, for example an Azure Container Apps job or a cron job.

## License

MIT. See [LICENSE](LICENSE).
