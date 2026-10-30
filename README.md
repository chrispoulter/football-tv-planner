# Football TV Planner

A full-stack app for browsing football on UK TV and streaming services and adding games to your calendar feed. .NET minimal API with EF Core and PostgreSQL, a React SPA, and Aspire for local development.

## Features

- **Fixtures by day.** Pick one of the next 14 days, then filter by competition or channel. Kick-off times, and which games fall on which day, use the viewer's local time zone.
- **My Schedule.** Star a game to add it to your schedule. The **My Schedule** filter narrows the list to your starred games.
- **Add to calendar.** Each game has links for Google Calendar and Outlook.com, plus an `.ics` download. Events include a reminder at kick-off.
- **Calendar feed.** Each user gets a private `webcal://` feed of their starred games, with one-click subscribe for Google and Outlook. Resetting the link stops the old one from working.
- **Accounts.** Email and password registration with email confirmation and password reset, optional Google sign-in, linking Google to an existing account, two-factor authentication with recovery codes, and account deletion.

## Projects

| Project                              | Description                                                                                                     |
| ------------------------------------ | --------------------------------------------------------------------------------------------------------------- |
| `FootballTvPlanner.Server`           | .NET 10 minimal API with Identity cookie auth, EF Core and Scalar at `/scalar`. Also serves the built client.   |
| `FootballTvPlanner.FixtureSync`      | Console job that pulls fixtures from a provider into the `fixtures` table, then exits. Run it on a schedule.    |
| `FootballTvPlanner.Data`             | EF Core DbContext, entities and migrations, shared by the server, the fixture sync and the migration service.   |
| `FootballTvPlanner.MigrationService` | Console job that applies pending EF Core migrations to the database, then exits. Runs before the server starts. |
| `FootballTvPlanner.Client`           | React 19 app built with Vite, React Router, TanStack Query, shadcn/ui and Tailwind CSS.                         |
| `FootballTvPlanner.AppHost`          | Aspire app host for local development.                                                                          |
| `FootballTvPlanner.ServiceDefaults`  | Shared Aspire setup: OpenTelemetry, health checks, service discovery and resilience.                            |

## Getting started

### Prerequisites

- .NET SDK 10 (see `global.json`)
- Node.js 26
- Docker, which Aspire uses to run PostgreSQL and Mailpit

### Run the app

```
dotnet run --project FootballTvPlanner.AppHost
```

Aspire starts PostgreSQL, Mailpit, the migration service, the server (`server`), the fixture sync and the Vite dev server (`client`). When they're up:

- Client: http://localhost:5173
- API docs (Scalar): the `server` resource's link in the Aspire dashboard
- Mailpit, which receives confirmation and password reset emails: http://localhost:8025

The migration service applies migrations and exits, and the server starts once it has finished. The fixture sync only needs the migrated database, not the server. Start the `fixture-sync` resource from the dashboard to pull fixtures, and restart it to pick up new ones.

> PostgreSQL (5432) and Mailpit (1025 and 8025) use fixed host ports and persistent containers. Stop anything else that uses those ports, or change them in `FootballTvPlanner.AppHost/AppHost.cs`.

You can also run everything with `docker compose up`, which builds the three app images (server, migrations and fixture sync) and serves the app on http://localhost:5257. The apps send logs, traces and metrics to a standalone Aspire dashboard at http://localhost:18888.

## Configuration

`appsettings.json` in each project lists every setting, with blank values where you need to supply your own. To override them locally, create an `appsettings.Development.json` next to it. Git ignores these files. You can also use user secrets (the server has these set up) or environment variables such as `Authentication__Google__ClientId`.

### Google sign-in

Google sign-in is optional. It's turned on only when `Authentication:Google:ClientId` is set.

1. In Google Cloud Console, go to **APIs & Services → Credentials** and create an OAuth client ID of type **Web application**.
2. Add these authorised redirect URIs. The callback always goes through the site's origin (Vite proxies `/api` in development):
   - `http://localhost:5173/api/signin-google`
   - `https://<site host>/api/signin-google`
3. Put the credentials in `FootballTvPlanner.Server/appsettings.Development.json`:

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
| `Web`            | Scrapes fixtures from the page at `FixtureSync:Web:Url`, which is required.                                            |

Set `FixtureSync:Provider` in `FootballTvPlanner.FixtureSync/appsettings.Development.json` to switch.

### Email

The server sends email over SMTP using the `Mail` connection string, for example `Endpoint=smtp://localhost:1025`. `Email:FromAddress` and `Email:FromName` are the sender address and display name. `Email:SiteUrl` is the site's URL used for links in emails. Aspire sets `Email:SiteUrl` for you.

### Client runtime config

Any server setting whose name starts with `VITE_`, such as the environment variable `VITE_FEATURE_X`, is sent to the client at runtime. `index.html` loads `/env.js` (mapped in `ClientConfigExtensions`), which sets `window.__ENV__`, and `FootballTvPlanner.Client/src/lib/config.ts` merges it over the build-time `import.meta.env` and validates it. You can change these values without rebuilding the image. Add new keys to the schema in `config.ts`.

## How it works

### Authentication and hosting

In production the server serves the client's static files itself, so the SPA and every API endpoint (mapped under `/api`) share one origin. This keeps the Identity cookie first-party. In development, Vite proxies `/api/*` and `/env.js` to the server (`API_URL`).

The Vite proxy in development, and the hosting ingress in production, send `X-Forwarded-*` headers. The server reads them in `ForwardedHeadersExtensions` so it can build correct external login redirect URIs. Unknown `/api/*` routes return 404, and any other unknown path falls back to `index.html` for client-side routing.

Static files are served by `StaticFileExtensions`. Files under `/assets` have content-hashed names, so they're cached for a year as `immutable`. Everything else, including `index.html`, is sent with `Cache-Control: no-cache`, so browsers revalidate it and pick up a new deploy straight away. `/env.js` is sent with `Cache-Control: no-store`, so a changed `VITE_*` setting reaches browsers without a deploy, and Cloudflare doesn't cache it.

### Dates and times

Every time is stored and returned in UTC: `timestamptz` in Postgres, ISO 8601 strings in the API, and UTC `DTSTART` values in `.ics` files. The client converts them to the viewer's time zone (`FootballTvPlanner.Client/src/lib/date-time.ts`). When you pick a day, the client sends it to `GET /api/fixtures` as a UTC `from`/`to` range, so the API doesn't need to know the viewer's time zone. The only code that knows about UK time is in the fixture providers, which convert UK kick-off times to UTC with `DateTimeExtensions.ToUtc`.

### Fixture sync

Each run, `FixtureSyncer`:

- upserts fixtures by `(Source, ExternalId)`
- replaces each fixture's channel list
- deletes upcoming fixtures from the same source that the provider no longer lists, because a moved game comes back under a new ID

If a provider returns nothing, the run fails instead of deleting every upcoming fixture.

The sync uses the shared `FootballTvPlannerDbContext` and ignores its query filters, so hidden competitions and channels, and the fixtures they hide, are still updated.

To add a provider:

1. Implement `IFixtureProvider` with a unique `Source` and external IDs that don't change between runs.
2. Register it in `FixtureProviderExtensions.AddFixtureProvider` under a new `FixtureSync:Provider` name.

## Development

### Database migrations

Migrations are in `FootballTvPlanner.Data/Migrations`, next to the DbContext. To add one:

```
dotnet tool restore
dotnet ef migrations add <Name> --project FootballTvPlanner.Data --startup-project FootballTvPlanner.Server
```

The server doesn't apply migrations itself. `FootballTvPlanner.MigrationService` applies them: through Aspire locally, as a `docker compose` service that the server waits for, and as an Azure Container Apps job that the deploy pipeline starts in production.

### Formatting and linting

```
dotnet csharpier format .

cd FootballTvPlanner.Client
npm run lint
npm run format
```

## Deployment

On every push to `main`, `develop`, `feature/**`, `release/**` and `hotfix/**`, GitHub Actions builds and lints everything and pushes three images to GitHub Container Registry: `-server`, `-fixture-sync` and `-migrations`.

In production:

- **Migrations:** set `ConnectionStrings__Database` and run it as a manually triggered job, for example an Azure Container Apps job named by the `AZURE_MIGRATIONS_JOB` variable. The deploy pipeline updates the job's image, starts it and waits for that execution to succeed before updating the other containers. If the execution fails, or the step doesn't finish within 4 minutes, the deploy stops. The job's replica timeout (120s) must stay below that limit.
- **Server:** run the `-server` image as a container app with external ingress on port 8080, for example the Azure Container App named by the `AZURE_SERVER_APP` variable. It serves both the API and the client. Set `ConnectionStrings__Database`, `ConnectionStrings__Mail`, `Email__SiteUrl` (the site's public URL), `Email__FromAddress` and, optionally, `Authentication__Google__*` and any `VITE_*` client settings.
- **Fixture sync:** set `ConnectionStrings__Database` and `FixtureSync__Provider` (plus `FixtureSync__Web__Url` for the `Web` provider), and run it as a scheduled job, for example an Azure Container Apps job or a cron job.

## License

MIT. See [LICENSE](LICENSE).
