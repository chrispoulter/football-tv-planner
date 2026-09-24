namespace SoccerTv.Api.Common.Authentication;

public interface IJwtService
{
    public string GenerateJwtToken(IJwtUser user);
}
