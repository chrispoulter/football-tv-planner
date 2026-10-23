namespace FootballTvPlanner.Server.Features.Profile.DownloadPersonalData;

public record PersonalDataResponse(
    Guid Id,
    string? Name,
    string? Email,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    IEnumerable<string> LinkedAccounts,
    string? CalendarFeedToken,
    IEnumerable<PersonalDataBookmark> BookmarkedFixtures
);

public record PersonalDataBookmark(
    string HomeTeam,
    string AwayTeam,
    string Competition,
    DateTimeOffset KickoffUtc,
    DateTimeOffset BookmarkedAt
);
