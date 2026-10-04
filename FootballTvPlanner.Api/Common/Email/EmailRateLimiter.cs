using System.Threading.RateLimiting;

namespace FootballTvPlanner.Api.Common.Email;

public sealed class EmailRateLimiter : IDisposable
{
    private readonly PartitionedRateLimiter<string> _limiter = PartitionedRateLimiter.Create<
        string,
        string
    >(address =>
        RateLimitPartition.GetFixedWindowLimiter(
            address.Trim().ToUpperInvariant(),
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromHours(1),
                QueueLimit = 0,
            }
        )
    );

    public bool TryAcquire(string address)
    {
        using var lease = _limiter.AttemptAcquire(address);

        return lease.IsAcquired;
    }

    public void Dispose() => _limiter.Dispose();
}
