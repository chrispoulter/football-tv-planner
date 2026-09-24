using NpgsqlTypes;
using SoccerTv.Api.Common.Authentication;

namespace SoccerTv.Api.Data.Users;

public class User : IJwtUser
{
    public Guid Id { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string NormalizedEmailAddress { get; } = null!;

    public string? Password { get; set; }

    public string? PasswordResetToken { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public bool IsLockedOut { get; set; }

    public string? CalendarFeedToken { get; set; }

    public int ReminderMinutesBefore { get; set; } = 30;

    public NpgsqlTsVector SearchVector { get; } = null!;
}
