namespace FootballTvPlanner.Server.Features.Profile.LinkedAccounts.GetLinkedAccounts;

public record LinkedAccountsResponse(IEnumerable<string> Providers);
