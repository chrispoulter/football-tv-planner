using System.Buffers.Text;
using System.Security.Cryptography;

namespace SoccerTv.Api.Features.Schedule;

public record CalendarFeedResponse(string HttpsUrl, string WebcalUrl)
{
    public static string GenerateToken() =>
        Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));

    public static CalendarFeedResponse Create(HttpRequest request, string token)
    {
        var url = $"{request.Scheme}://{request.Host}{request.PathBase}/calendar/{token}.ics";
        var webcalUrl = $"webcal://{request.Host}{request.PathBase}/calendar/{token}.ics";

        return new CalendarFeedResponse(url, webcalUrl);
    }
}
