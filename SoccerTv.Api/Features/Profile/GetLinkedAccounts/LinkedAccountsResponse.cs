namespace SoccerTv.Api.Features.Account.GetLinkedAccounts;

public record LinkedAccountsResponse(IEnumerable<LinkedAccount> Accounts);

/// <summary>
/// One of the available external login providers, and whether the current user has linked it.
/// </summary>
public record LinkedAccount(string Provider, string DisplayName, bool IsLinked);
