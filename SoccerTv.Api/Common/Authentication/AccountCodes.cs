using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace SoccerTv.Api.Common.Authentication;

/// <summary>
/// Identity tokens are Base64 with characters that aren't safe in a URL, so they're sent in links
/// Base64Url-encoded.
/// </summary>
public static class AccountCodes
{
    public static string Encode(string token) =>
        WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    public static string? Decode(string code)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return null;
        }
    }

    // Authenticator apps often show the code in groups, e.g. "123 456"
    public static string NormalizeAuthenticatorCode(string code) =>
        code.Replace(" ", string.Empty).Replace("-", string.Empty);

    public static string NormalizeRecoveryCode(string code) => code.Replace(" ", string.Empty);
}
