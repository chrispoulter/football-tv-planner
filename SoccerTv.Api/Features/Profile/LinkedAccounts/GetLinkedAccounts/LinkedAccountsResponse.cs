namespace SoccerTv.Api.Features.Profile.LinkedAccounts.GetLinkedAccounts;

public record LinkedAccountsResponse(IEnumerable<LinkedAccount> Accounts);

public record LinkedAccount(string Provider, string DisplayName, bool IsLinked);
