using System.Text.RegularExpressions;
using FluentValidation;

namespace FootballTvPlanner.Server.Common.Validation;

public static partial class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string?> PlainEmailAddress<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    ) =>
        ruleBuilder
            .Must(email => email is null || PlainEmailAddressRegex().IsMatch(email))
            .WithMessage("'{PropertyName}' is not a valid email address.");

    [GeneratedRegex(
        @"^(?!\.)(?!.*\.\.)([A-Za-z0-9_'+\-\.]*)[A-Za-z0-9_+-]@([A-Za-z0-9][A-Za-z0-9\-]*\.)+[A-Za-z]{2,}\z",
        RegexOptions.CultureInvariant
    )]
    private static partial Regex PlainEmailAddressRegex();
}
