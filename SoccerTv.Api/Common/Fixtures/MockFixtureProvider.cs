using System.Globalization;
using SoccerTv.Api.Common.Time;
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
        string[] Teams,
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

    private static readonly ProviderChannel SkyMainEvent = new("Sky Sports Main Event", 10);
    private static readonly ProviderChannel SkyPremierLeague = new("Sky Sports Premier League", 11);
    private static readonly ProviderChannel SkyFootball = new("Sky Sports Football", 12);
    private static readonly ProviderChannel SkyPlus = new("Sky Sports+", 13);
    private static readonly ProviderChannel Tnt1 = new("TNT Sports 1", 20);
    private static readonly ProviderChannel Tnt2 = new("TNT Sports 2", 21);
    private static readonly ProviderChannel PrimeVideo = new("Amazon Prime Video", 30);
    private static readonly ProviderChannel BbcTwo = new("BBC Two", 40);
    private static readonly ProviderChannel BbcIplayer = new("BBC iPlayer", 41);
    private static readonly ProviderChannel Premier1 = new("Premier Sports 1", 60);
    private static readonly ProviderChannel Premier2 = new("Premier Sports 2", 61);

    private static readonly MockCompetition[] Competitions =
    [
        new(
            Code: "epl",
            Competition: new("Premier League", 10),
            Teams:
            [
                "Arsenal",
                "Aston Villa",
                "Bournemouth",
                "Brentford",
                "Brighton & Hove Albion",
                "Burnley",
                "Chelsea",
                "Crystal Palace",
                "Everton",
                "Fulham",
                "Leeds United",
                "Liverpool",
                "Manchester City",
                "Manchester United",
                "Newcastle United",
                "Nottingham Forest",
                "Sunderland",
                "Tottenham Hotspur",
                "West Ham United",
                "Wolverhampton Wanderers",
            ],
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
            Competition: new("Championship", 20),
            Teams:
            [
                "Birmingham City",
                "Blackburn Rovers",
                "Bristol City",
                "Charlton Athletic",
                "Coventry City",
                "Derby County",
                "Hull City",
                "Ipswich Town",
                "Leicester City",
                "Middlesbrough",
                "Millwall",
                "Norwich City",
                "Oxford United",
                "Portsmouth",
                "Preston North End",
                "Queens Park Rangers",
                "Sheffield United",
                "Sheffield Wednesday",
                "Southampton",
                "Stoke City",
                "Swansea City",
                "Watford",
                "West Bromwich Albion",
                "Wrexham",
            ],
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
            Competition: new("UEFA Champions League", 30),
            Teams:
            [
                "Arsenal",
                "Liverpool",
                "Manchester City",
                "Chelsea",
                "Tottenham Hotspur",
                "Newcastle United",
                "Real Madrid",
                "Barcelona",
                "Bayern Munich",
                "Paris Saint-Germain",
                "Inter Milan",
                "Borussia Dortmund",
                "Juventus",
                "Benfica",
                "PSV Eindhoven",
                "Atlético Madrid",
                "Napoli",
                "Celtic",
            ],
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
            Competition: new("UEFA Europa League", 40),
            Teams:
            [
                "Aston Villa",
                "Nottingham Forest",
                "Rangers",
                "Roma",
                "Lyon",
                "Porto",
                "Fenerbahçe",
                "AZ Alkmaar",
                "Real Betis",
                "Eintracht Frankfurt",
                "Lille",
                "Braga",
            ],
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
            Competition: new("Scottish Premiership", 50),
            Teams:
            [
                "Aberdeen",
                "Celtic",
                "Dundee",
                "Dundee United",
                "Falkirk",
                "Heart of Midlothian",
                "Hibernian",
                "Kilmarnock",
                "Livingston",
                "Motherwell",
                "Rangers",
                "St Mirren",
            ],
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
            Competition: new("Women's Super League", 60),
            Teams:
            [
                "Arsenal Women",
                "Aston Villa Women",
                "Brighton & Hove Albion Women",
                "Chelsea Women",
                "Everton Women",
                "Leicester City Women",
                "Liverpool Women",
                "London City Lionesses",
                "Manchester City Women",
                "Manchester United Women",
                "Tottenham Hotspur Women",
                "West Ham United Women",
            ],
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
            Competition: new("La Liga", 70),
            Teams:
            [
                "Real Madrid",
                "Barcelona",
                "Atlético Madrid",
                "Athletic Club",
                "Real Sociedad",
                "Villarreal",
                "Real Betis",
                "Sevilla",
                "Valencia",
                "Girona",
                "Celta Vigo",
                "Osasuna",
            ],
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
