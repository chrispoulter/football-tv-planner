using Microsoft.Extensions.Options;

namespace FootballTvPlanner.FixtureSync.Providers.Mock;

/// <summary>
/// Generates random televised fixtures from a fixed list of competitions so the app can be
/// developed without a licensed data feed. Each date is seeded from its day number, so
/// re-syncing the same range produces the same fixtures.
/// </summary>
public class MockFixtureProvider(IOptions<MockSettings> settings, TimeProvider timeProvider)
    : IFixtureProvider
{
    private readonly MockSettings _settings = settings.Value;

    public string Source => "Mock";

    public Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        CancellationToken cancellationToken = default
    )
    {
        var fixtures = new List<ProviderFixture>();

        // Start a day back so every time zone's "today" is covered.
        var from = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddDays(-1);
        var to = from.AddDays(_settings.DaysAhead + 1);

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var random = new Random(date.DayNumber);

            foreach (var competition in Competitions)
            {
                var kickOffs = competition.KickOffs.Where(k => k.Day == date.DayOfWeek);

                foreach (var (_, time) in kickOffs)
                {
                    var teams = competition.Teams.ToArray();
                    random.Shuffle(teams);

                    fixtures.Add(
                        new ProviderFixture(
                            ExternalId: $"mock-{competition.Name}-{date:yyyyMMdd}-{time:HHmm}",
                            Competition: competition.Name,
                            HomeTeam: teams[0],
                            AwayTeam: teams[1],
                            KickoffUtc: UkTime.ToUtc(date, time),
                            Channels: [competition.Channel]
                        )
                    );
                }
            }
        }

        return Task.FromResult<IReadOnlyList<ProviderFixture>>(fixtures);
    }

    private record MockCompetition(
        string Name,
        ProviderChannel Channel,
        string[] Teams,
        (DayOfWeek Day, TimeOnly Time)[] KickOffs
    );

    private static readonly MockCompetition[] Competitions =
    [
        new(
            "Premier League",
            new("Sky Sports Main Event", 10),
            [
                "Arsenal",
                "Aston Villa",
                "Brighton & Hove Albion",
                "Chelsea",
                "Everton",
                "Liverpool",
                "Manchester City",
                "Manchester United",
                "Newcastle United",
                "Tottenham Hotspur",
            ],
            [
                (DayOfWeek.Friday, new(20, 0)),
                (DayOfWeek.Saturday, new(17, 30)),
                (DayOfWeek.Sunday, new(14, 0)),
                (DayOfWeek.Sunday, new(16, 30)),
                (DayOfWeek.Monday, new(20, 0)),
            ]
        ),
        new(
            "Championship",
            new("Sky Sports Football", 12),
            [
                "Birmingham City",
                "Coventry City",
                "Derby County",
                "Leicester City",
                "Middlesbrough",
                "Norwich City",
                "Sheffield United",
                "Southampton",
                "Watford",
                "Wrexham",
            ],
            [
                (DayOfWeek.Saturday, new(12, 30)),
                (DayOfWeek.Tuesday, new(19, 45)),
                (DayOfWeek.Wednesday, new(20, 0)),
            ]
        ),
        new(
            "UEFA Champions League",
            new("TNT Sports 1", 20),
            [
                "Arsenal",
                "Liverpool",
                "Manchester City",
                "Real Madrid",
                "Barcelona",
                "Bayern Munich",
                "Paris Saint-Germain",
                "Inter Milan",
            ],
            [
                (DayOfWeek.Tuesday, new(20, 0)),
                (DayOfWeek.Wednesday, new(17, 45)),
                (DayOfWeek.Wednesday, new(20, 0)),
            ]
        ),
        new(
            "Scottish Premiership",
            new("Premier Sports 1", 60),
            ["Aberdeen", "Celtic", "Hearts", "Hibernian", "Rangers", "St Mirren"],
            [(DayOfWeek.Sunday, new(12, 0))]
        ),
        new(
            "Women's Super League",
            new("BBC Two", 40),
            [
                "Arsenal Women",
                "Chelsea Women",
                "Liverpool Women",
                "Manchester City Women",
                "Manchester United Women",
            ],
            [(DayOfWeek.Saturday, new(12, 30))]
        ),
        new(
            "La Liga",
            new("Premier Sports 2", 61),
            ["Real Madrid", "Barcelona", "Atlético Madrid", "Sevilla", "Valencia", "Villarreal"],
            [(DayOfWeek.Saturday, new(20, 0)), (DayOfWeek.Sunday, new(20, 0))]
        ),
    ];
}
