using FluentValidation;
using SoccerTv.Api.Data.Users;

namespace SoccerTv.Api.Features.Account.UpdateProfile;

public record UpdateProfileRequest(string Name);

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(UserConfiguration.NameMaxLength);
    }
}
