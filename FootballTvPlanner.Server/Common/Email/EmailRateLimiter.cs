using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Email;

public sealed class EmailRateLimiter : IDisposable
{
    private readonly PartitionedRateLimiter<string> _limiter;

    public EmailRateLimiter(IOptions<EmailSettings> emailSettings)
    {
        var settings = emailSettings.Value; 
        
        _limiter = PartitionedRateLimiter.Create<string, string>(toAddress =>
            RateLimitPartition.GetSlidingWindowLimiter(
                toAddress.ToUpperInvariant(),
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = settings.RecipientRateLimit.PermitLimit,
                    Window = settings.RecipientRateLimit.Window,
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                }
            )
        );
    }

    public bool TryAcquire(string toAddress)
    {
        using var lease = _limiter.AttemptAcquire(toAddress);

        return lease.IsAcquired;
    }

    public void Dispose()
    {
        _limiter.Dispose();
    }
}
