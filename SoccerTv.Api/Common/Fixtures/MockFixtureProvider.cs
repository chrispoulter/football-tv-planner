using System.Globalization;
using SoccerTv.Api.Common.Time;
using SoccerTv.Api.Data.Channels;
using SoccerTv.Api.Data.Fixtures;

namespace SoccerTv.Api.Common.Fixtures;

/// <summary>
/// Generates realistic, deterministic UK televised fixtures so the app can be developed
/// without a licensed data feed. Kick-off slots and channels follow the current UK rights
/// holders, and no games are scheduled in the Saturday 15:00 blackout.
/// </summary>
public class MockFixtureProvider : IFixtureProvider
{
    public string Source => "Mock";

    public Task<IReadOnlyList<ProviderFixture>> GetFixturesAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default
    )
    {
        var fixtures = new List<ProviderFixture>();

        var anchors = Enumerable
            .Range(0, to.DayNumber - from.DayNumber + 1)
            .Select(offset => GetRoundAnchor(from.AddDays(offset)))
            .Distinct();

        foreach (var anchor in anchors)
        {
            var isWeekend = anchor.DayOfWeek == DayOfWeek.Friday;
            var isEuropeanWeek =
                ISOWeek.GetWeekOfYear(anchor.ToDateTime(TimeOnly.MinValue)) % 2 == 1;

            for (var i = 0; i < Competitions.Length; i++)
            {
                var competition = Competitions[i];

                var slots = isWeekend
                    ? competition.WeekendSlots
                    : competition.Midweek switch
                    {
                        MidweekWeeks.European when isEuropeanWeek => competition.MidweekSlots,
                        MidweekWeeks.Domestic when !isEuropeanWeek => competition.MidweekSlots,
                        _ => [],
                    };

                var random = new Random(anchor.DayNumber * 31 + i);
                var teams = competition.Teams.OrderBy(_ => random.Next()).ToArray();

                for (var j = 0; j < slots.Length; j++)
                {
                    var slot = slots[j];
                    var date = anchor.AddDays(((int)slot.Day - (int)anchor.DayOfWeek + 7) % 7);

                    if (date < from || date > to)
                    {
                        continue;
                    }

                    fixtures.Add(
                        new ProviderFixture(
                            ExternalId: $"mock-{competition.Code}-{date:yyyyMMdd}-{slot.Time:HHmm}-{j}",
                            Competition: competition.Competition,
                            HomeTeam: teams[2 * j],
                            AwayTeam: teams[2 * j + 1],
                            KickoffUtc: UkTime.ToUtc(date, slot.Time),
                            Venue: null,
                            Status: FixtureStatus.Scheduled,
                            Channels: slot.Channels
                        )
                    );
                }
            }
        }

        return Task.FromResult<IReadOnlyList<ProviderFixture>>(fixtures);
    }

    /// <summary>
    /// Weekend rounds run Friday to Monday and midweek rounds Tuesday to Thursday, so that
    /// a team only appears once per round.
    /// </summary>
    private static DateOnly GetRoundAnchor(DateOnly date) =>
        date.DayOfWeek switch
        {
            DayOfWeek.Friday => date,
            DayOfWeek.Saturday => date.AddDays(-1),
            DayOfWeek.Sunday => date.AddDays(-2),
            DayOfWeek.Monday => date.AddDays(-3),
            DayOfWeek.Tuesday => date,
            DayOfWeek.Wednesday => date.AddDays(-1),
            _ => date.AddDays(-2),
        };

    private enum MidweekWeeks
    {
        None,
        European,
        Domestic,
    }

    private record Slot(DayOfWeek Day, TimeOnly Time, ProviderChannel[] Channels);

    private record MockCompetition(
        string Code,
        ProviderCompetition Competition,
        ProviderTeam[] Teams,
        Slot[] WeekendSlots,
        Slot[] MidweekSlots,
        MidweekWeeks Midweek
    );

    private static Slot At(
        DayOfWeek day,
        int hour,
        int minute,
        params ProviderChannel[] channels
    ) => new(day, new TimeOnly(hour, minute), channels);

    private static ProviderTeam[] Teams(params string[] names) =>
        [
            .. names.Select(n =>
                n.Split('|') is [var name, var shortName]
                    ? new ProviderTeam(name, shortName)
                    : new ProviderTeam(n, n)
            ),
        ];

    private static readonly ProviderChannel SkyMainEvent = new(
        "Sky Sports Main Event",
        "Sky",
        ChannelType.Tv,
        10
    );
    private static readonly ProviderChannel SkyPremierLeague = new(
        "Sky Sports Premier League",
        "Sky",
        ChannelType.Tv,
        11
    );
    private static readonly ProviderChannel SkyFootball = new(
        "Sky Sports Football",
        "Sky",
        ChannelType.Tv,
        12
    );
    private static readonly ProviderChannel SkyPlus = new(
        "Sky Sports+",
        "Sky",
        ChannelType.Streaming,
        13
    );
    private static readonly ProviderChannel Tnt1 = new("TNT Sports 1", "TNT", ChannelType.Tv, 20);
    private static readonly ProviderChannel Tnt2 = new("TNT Sports 2", "TNT", ChannelType.Tv, 21);
    private static readonly ProviderChannel PrimeVideo = new(
        "Amazon Prime Video",
        "Amazon",
        ChannelType.Streaming,
        30
    );
    private static readonly ProviderChannel BbcTwo = new("BBC Two", "BBC", ChannelType.Tv, 40);
    private static readonly ProviderChannel BbcIplayer = new(
        "BBC iPlayer",
        "BBC",
        ChannelType.Streaming,
        41
    );
    private static readonly ProviderChannel Premier1 = new(
        "Premier Sports 1",
        "Premier Sports",
        ChannelType.Tv,
        60
    );
    private static readonly ProviderChannel Premier2 = new(
        "Premier Sports 2",
        "Premier Sports",
        ChannelType.Tv,
        61
    );

    private static readonly MockCompetition[] Competitions =
    [
        new(
            Code: "epl",
            Competition: new("Premier League", "PL", "England", 10),
            Teams: Teams(
                "Arsenal",
                "Aston Villa",
                "Bournemouth",
                "Brentford",
                "Brighton & Hove Albion|Brighton",
                "Burnley",
                "Chelsea",
                "Crystal Palace",
                "Everton",
                "Fulham",
                "Leeds United|Leeds",
                "Liverpool",
                "Manchester City|Man City",
                "Manchester United|Man Utd",
                "Newcastle United|Newcastle",
                "Nottingham Forest|Nott'm Forest",
                "Sunderland",
                "Tottenham Hotspur|Spurs",
                "West Ham United|West Ham",
                "Wolverhampton Wanderers|Wolves"
            ),
            WeekendSlots:
            [
                At(DayOfWeek.Friday, 20, 0, SkyMainEvent, SkyPremierLeague),
                At(DayOfWeek.Saturday, 12, 30, Tnt1),
                At(DayOfWeek.Saturday, 17, 30, SkyMainEvent, SkyPremierLeague),
                At(DayOfWeek.Sunday, 14, 0, SkyPremierLeague),
                At(DayOfWeek.Sunday, 16, 30, SkyMainEvent, SkyPremierLeague),
                At(DayOfWeek.Monday, 20, 0, SkyMainEvent, SkyPremierLeague),
            ],
            MidweekSlots: [],
            Midweek: MidweekWeeks.None
        ),
        new(
            Code: "efl-champ",
            Competition: new("Championship", "Champ", "England", 20),
            Teams: Teams(
                "Birmingham City|Birmingham",
                "Blackburn Rovers|Blackburn",
                "Bristol City",
                "Charlton Athletic|Charlton",
                "Coventry City|Coventry",
                "Derby County|Derby",
                "Hull City|Hull",
                "Ipswich Town|Ipswich",
                "Leicester City|Leicester",
                "Middlesbrough",
                "Millwall",
                "Norwich City|Norwich",
                "Oxford United|Oxford",
                "Portsmouth",
                "Preston North End|Preston",
                "Queens Park Rangers|QPR",
                "Sheffield United|Sheff Utd",
                "Sheffield Wednesday|Sheff Wed",
                "Southampton",
                "Stoke City|Stoke",
                "Swansea City|Swansea",
                "Watford",
                "West Bromwich Albion|West Brom",
                "Wrexham"
            ),
            WeekendSlots:
            [
                At(DayOfWeek.Friday, 20, 0, SkyFootball),
                At(DayOfWeek.Saturday, 12, 30, SkyFootball),
                At(DayOfWeek.Saturday, 17, 30, SkyPlus),
                At(DayOfWeek.Sunday, 12, 0, SkyFootball),
            ],
            MidweekSlots:
            [
                At(DayOfWeek.Tuesday, 19, 45, SkyFootball),
                At(DayOfWeek.Tuesday, 19, 45, SkyPlus),
                At(DayOfWeek.Wednesday, 20, 0, SkyFootball),
                At(DayOfWeek.Wednesday, 19, 45, SkyPlus),
            ],
            Midweek: MidweekWeeks.Domestic
        ),
        new(
            Code: "ucl",
            Competition: new("UEFA Champions League", "UCL", "Europe", 30),
            Teams: Teams(
                "Arsenal",
                "Liverpool",
                "Manchester City|Man City",
                "Chelsea",
                "Tottenham Hotspur|Spurs",
                "Newcastle United|Newcastle",
                "Real Madrid",
                "Barcelona",
                "Bayern Munich",
                "Paris Saint-Germain|PSG",
                "Inter Milan|Inter",
                "Borussia Dortmund|Dortmund",
                "Juventus",
                "Benfica",
                "PSV Eindhoven|PSV",
                "Atlético Madrid|Atlético",
                "Napoli",
                "Celtic"
            ),
            WeekendSlots: [],
            MidweekSlots:
            [
                At(DayOfWeek.Tuesday, 17, 45, Tnt1),
                At(DayOfWeek.Tuesday, 20, 0, PrimeVideo),
                At(DayOfWeek.Tuesday, 20, 0, Tnt2),
                At(DayOfWeek.Wednesday, 17, 45, Tnt1),
                At(DayOfWeek.Wednesday, 20, 0, Tnt1),
                At(DayOfWeek.Wednesday, 20, 0, Tnt2),
            ],
            Midweek: MidweekWeeks.European
        ),
        new(
            Code: "uel",
            Competition: new("UEFA Europa League", "UEL", "Europe", 40),
            Teams: Teams(
                "Aston Villa",
                "Nottingham Forest|Nott'm Forest",
                "Rangers",
                "Roma",
                "Lyon",
                "Porto",
                "Fenerbahçe",
                "AZ Alkmaar|AZ",
                "Real Betis|Betis",
                "Eintracht Frankfurt|Frankfurt",
                "Lille",
                "Braga"
            ),
            WeekendSlots: [],
            MidweekSlots:
            [
                At(DayOfWeek.Thursday, 17, 45, Tnt2),
                At(DayOfWeek.Thursday, 20, 0, Tnt1),
                At(DayOfWeek.Thursday, 20, 0, Tnt2),
            ],
            Midweek: MidweekWeeks.European
        ),
        new(
            Code: "spfl",
            Competition: new("Scottish Premiership", "SPFL", "Scotland", 50),
            Teams: Teams(
                "Aberdeen",
                "Celtic",
                "Dundee",
                "Dundee United|Dundee Utd",
                "Falkirk",
                "Heart of Midlothian|Hearts",
                "Hibernian|Hibs",
                "Kilmarnock",
                "Livingston",
                "Motherwell",
                "Rangers",
                "St Mirren"
            ),
            WeekendSlots:
            [
                At(DayOfWeek.Saturday, 12, 30, SkyFootball),
                At(DayOfWeek.Sunday, 12, 0, SkyMainEvent),
            ],
            MidweekSlots: [],
            Midweek: MidweekWeeks.None
        ),
        new(
            Code: "wsl",
            Competition: new("Women's Super League", "WSL", "England", 60),
            Teams: Teams(
                "Arsenal Women",
                "Aston Villa Women",
                "Brighton & Hove Albion Women|Brighton Women",
                "Chelsea Women",
                "Everton Women",
                "Leicester City Women",
                "Liverpool Women",
                "London City Lionesses",
                "Manchester City Women|Man City Women",
                "Manchester United Women|Man Utd Women",
                "Tottenham Hotspur Women|Spurs Women",
                "West Ham United Women|West Ham Women"
            ),
            WeekendSlots:
            [
                At(DayOfWeek.Saturday, 12, 30, BbcTwo, BbcIplayer),
                At(DayOfWeek.Sunday, 18, 45, SkyFootball),
            ],
            MidweekSlots: [],
            Midweek: MidweekWeeks.None
        ),
        new(
            Code: "laliga",
            Competition: new("La Liga", "LaLiga", "Spain", 70),
            Teams: Teams(
                "Real Madrid",
                "Barcelona",
                "Atlético Madrid|Atlético",
                "Athletic Club",
                "Real Sociedad",
                "Villarreal",
                "Real Betis|Betis",
                "Sevilla",
                "Valencia",
                "Girona",
                "Celta Vigo|Celta",
                "Osasuna"
            ),
            WeekendSlots:
            [
                At(DayOfWeek.Saturday, 17, 30, Premier2),
                At(DayOfWeek.Saturday, 20, 0, Premier1),
                At(DayOfWeek.Sunday, 20, 0, Premier1),
            ],
            MidweekSlots: [],
            Midweek: MidweekWeeks.None
        ),
    ];
}
