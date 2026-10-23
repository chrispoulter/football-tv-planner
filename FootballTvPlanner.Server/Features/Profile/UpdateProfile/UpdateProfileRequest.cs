using FluentValidation;
using FootballTvPlanner.Data.Users;

namespace FootballTvPlanner.Server.Features.Profile.UpdateProfile;

public record UpdateProfileRequest(string Name);

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(UserConfiguration.NameMaxLength);
    }
}
