using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Email;

public sealed class EmailRateLimiter(IOptions<EmailSettings> emailSettings) : IDisposable
{
    private readonly PartitionedRateLimiter<string> limiter = PartitionedRateLimiter.Create<
        string,
        string
    >(toAddress =>
        RateLimitPartition.GetSlidingWindowLimiter(
            toAddress.ToUpperInvariant(),
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = emailSettings.Value.RecipientPermitLimit,
                Window = emailSettings.Value.RecipientWindow,
                SegmentsPerWindow = 6,
                QueueLimit = 0,
            }
        )
    );

    public bool TryAcquire(string toAddress)
    {
        using var lease = limiter.AttemptAcquire(toAddress);

        return lease.IsAcquired;
    }

    public void Dispose() => limiter.Dispose();
}
