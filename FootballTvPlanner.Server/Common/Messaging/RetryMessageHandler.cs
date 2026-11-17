using System.Globalization;
using Rebus.Bus;
using Rebus.Exceptions;
using Rebus.Handlers;
using Rebus.Retry.Simple;

namespace FootballTvPlanner.Server.Common.Messaging;

public class RetryMessageHandler(IBus bus, ILogger<RetryMessageHandler> logger)
    : IHandleMessages<IFailed<object>>
{
    public const string RetryAttemptHeader = "x-retry-attempt";

    private static readonly TimeSpan[] Delays =
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15),
    ];

    public async Task Handle(IFailed<object> message)
    {
        var attempt = message.Headers.TryGetValue(RetryAttemptHeader, out var value)
            ? int.Parse(value, CultureInfo.InvariantCulture)
            : 0;

        if (attempt >= Delays.Length)
        {
            await bus.Advanced.TransportMessage.Deadletter(message.ErrorDescription);
            return;
        }

        var delay = Delays[attempt];

        logger.LogWarning(
            "Message {MessageType} failed, retrying in {Delay} (retry {Retry} of {MaxRetries})",
            message.Message.GetType().Name,
            delay,
            attempt + 1,
            Delays.Length
        );

        await bus.Advanced.TransportMessage.Defer(
            delay,
            new Dictionary<string, string>
            {
                [RetryAttemptHeader] = (attempt + 1).ToString(CultureInfo.InvariantCulture),
            }
        );
    }
}
