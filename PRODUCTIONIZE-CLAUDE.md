This is a well-built app. The structure is clean, with vertical-slice endpoints, separate jobs for migrations and fixture sync, and Aspire for local development. A lot of production work is already done: OpenTelemetry with Azure Monitor, problem details, rate limiting, data protection keys stored in the database, cache headers on static files, a non-root container, migrations that must succeed before the deploy continues, OIDC login to Azure, and the sync refusing to run when the provider returns nothing.

The two biggest gaps are that there are no tests at all, and one auth flow lets a stranger get into someone else's account.

## High priority

**1. A stranger can set up an account in someone else's name, then get into it once they sign in with Google.** [ExternalLoginCallbackEndpoint.cs:81](FootballTvPlanner.Server/Features/Account/ExternalLogin/ExternalLoginCallbackEndpoint.cs#L81)
- Registering never sends a confirmation email, so anyone can register `victim@gmail.com` with a password they choose.
- When the real owner later clicks "Sign in with Google", the callback finds that account by email, sets `EmailConfirmed = true` and links Google to it.
- The stranger's password still works, so they now share the real owner's account.
- **Fix:** if the existing account's email isn't confirmed, either remove its password and reset its security stamp before linking, or refuse to link automatically. Also check Google's `email_verified` claim.

**2. Registration doesn't send a confirmation email.** [RegisterEndpoint.cs](FootballTvPlanner.Server/Features/Account/Register/RegisterEndpoint.cs)
- The README promises "registration with email confirmation", but only changing your email sends one.
- Either send it on registration and add a resend option, or change the README.
- Registration also leaks which emails have accounts. Only `DuplicateUserName` is filtered out at [line 38](FootballTvPlanner.Server/Features/Account/Register/RegisterEndpoint.cs#L38), so the `DuplicateEmail` error ("Email 'x' is already taken") still reaches the client. That undoes the care taken in `ChangeEmailEndpoint` not to reveal this.

**3. There are no tests.** CI runs `dotnet test` against zero test projects, and the client has no test script. The code most worth testing first:
- `FixtureSyncer`: the upsert and delete logic.
- `DateTimeExtensions.ToUtc`: the clock-change edge cases.
- `WebFixtureProvider`: parsing against a saved copy of the HTML page, since a scraper breaks quietly when the site changes.
- `ExternalLoginRedirects.LocalOrRoot`.
- The auth flows, end to end, using `WebApplicationFactory` with a Postgres Testcontainer.

**4. The fixture sync can wipe users' starred games.** [FixtureSyncer.cs:25](FootballTvPlanner.FixtureSync/FixtureSyncer.cs#L25) and [:116](FootballTvPlanner.FixtureSync/FixtureSyncer.cs#L116)
- The sync only refuses to run when the provider returns nothing. If the scraped page partly breaks and returns, say, 3 games, every other upcoming fixture is deleted.
- Deleting a fixture also deletes every user's star on it (cascade).
- The web provider's external ID includes the match date ([WebFixtureProvider.cs:98](FootballTvPlanner.FixtureSync/Providers/Web/WebFixtureProvider.cs#L98)). So when a game moves to another day, users' stars on it disappear and the game drops out of their calendar feed without warning.
- **Fix:**
  - Refuse to delete when the run would remove more than a set share of upcoming fixtures.
  - Consider marking fixtures as removed instead of deleting them.
  - Match moved games on teams and competition so stars carry over.

## Medium priority

**5. The return-URL check may allow an open redirect.** [ExternalLoginRedirects.cs:10](FootballTvPlanner.Server/Features/ExternalLoginRedirects.cs#L10)
- The hand-written check blocks `//evil` and `/\evil`, but not control characters.
- With `returnUrl=/%09/evil.com`, the redirect target would be `/<tab>/evil.com`. Browsers strip the tab and go to `//evil.com`.
- ASP.NET's own `IsLocalUrl` rejects control characters for this reason. I haven't tested whether Kestrel lets the tab through, so it's worth a quick check. Either way, rejecting `char.IsControl` costs one line.

**6. The rate limits are too loose for logins and too strict for the calendar feed.** [Program.cs:40](FootballTvPlanner.Server/Program.cs#L40)
- One policy (100 requests a minute) covers every endpoint.
- **Logins:** login, 2FA, register and forgot-password can each be tried 100 times a minute per IP. Account lockout protects a single account, but not someone trying stolen passwords across many accounts. Add a stricter `Auth` policy, around 5–10 a minute.
- **Calendar feed:** `/api/calendar/{token}.ics` is fetched by Google and Outlook from a small number of shared IPs. As users grow, those fetchers will hit the per-IP limit and feeds will start failing with 429s. Count that endpoint per token, or leave it out of the limit.
- The rate limits and the per-recipient email limits are also kept in memory. They reset on every deploy and multiply by the number of server instances once it scales out.

**7. Email is sent while the request waits.** [ForgotPasswordEndpoint.cs:42](FootballTvPlanner.Server/Features/Account/ForgotPassword/ForgotPasswordEndpoint.cs#L42)
- Forgot-password responds in milliseconds for an unknown email but takes the full email-sending time for a real one. That reveals which emails have accounts, even though the response body is the same.
- A failed send is logged and swallowed, and nothing tries again.
- Moving sends to a background queue (a `Channel<T>` read by a `BackgroundService`) fixes both, and makes those endpoints faster.

**8. There are no security headers.** There's no CSP, HSTS, `X-Content-Type-Options`, `Referrer-Policy` or `frame-ancestors`. That matters for a cookie-authenticated app with pages like account deletion. Cloudflare can add HSTS, but the app has to set the CSP. Note that `/env.js` is an inline-config pattern, so a strict `script-src 'self'` still works.

**9. The data protection keys are stored unencrypted in Postgres.** [DataProtectionExtensions.cs:17](FootballTvPlanner.Server/Common/Infrastructure/DataProtectionExtensions.cs#L17) Anyone who can read the database can forge sign-in cookies for any user. Add `.ProtectKeysWithAzureKeyVault(...)` to encrypt them.

**10. The health check never checks the database.** [Extensions.cs:144](FootballTvPlanner.ServiceDefaults/Extensions.cs#L144) Only the `self` check is registered, so `/health` reports healthy while the database is down. Add `AddDbContextCheck<FootballTvPlannerDbContext>()` and point the Container App's readiness probe at it.

## Production setup gaps

- **No infrastructure as code.** The Container App, both jobs, the database, secrets, probes, scaling and the sync schedule all live only in Azure. Bicep, Terraform or `azd` would make the environment reproducible and reviewable. The repo already uses Aspire, so `aspire publish`/`azd` is the easiest route.
- **No checks after a deploy.** `az containerapp update` returns before the new revision is known to be healthy. Add a check against `/health` on the new revision, and a rollback step or traffic split if it fails.
- **Every migration must work with the old code too.** Migrations run before the new server starts, so the old code briefly runs against the new schema. That's fine if you split risky changes into add-then-remove steps, but it isn't written down anywhere.
- **CI checks are missing.**
  - No CSharpier check (`dotnet csharpier check .`) and no Prettier check.
  - No Dependabot or Renovate, and no CodeQL or `npm audit`. You're already overriding a vulnerable MimeKit dependency by hand.
- **Smaller items:**
  - Scalar and OpenAPI are public in production.
  - `AllowedHosts: "*"` is combined with trusting `X-Forwarded-Host` from any proxy.
  - The Dockerfile exposes port 8081, which isn't used.
  - `WhenWritingDefault` leaves `false` and `0` out of responses, which doesn't match the OpenAPI contract.
  - The fixtures cache lasts 1 hour, but the sync runs in a different process and can't clear it. Newly synced games can take up to an hour to appear, and deleted ones can still be shown and then return 404 when starred. A shorter expiry would make this less noticeable.

## What I'd do first

1. Fix the Google account-linking flaw and the registration leak (1, 2).
2. Add a test project and cover the sync, the time conversion and the redirect check (3, 4, 5).
3. Add the sync deletion safeguard (4).
4. Add a strict rate limit for auth endpoints and separate limits for the calendar feed (6).
5. Move email to a background queue (7).
6. Add security headers, Key Vault protection for the keys, and a database health check (8–10).
7. Move the infrastructure into code and check the deploy afterwards.

I can fix any of these. Items 1, 4 and 5 are each small changes. I can also turn this review into a shareable page if you want to pass it on.