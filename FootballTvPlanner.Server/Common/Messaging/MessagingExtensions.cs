using System.Reflection;
using Rebus.Config;
using Rebus.OpenTelemetry.Configuration;
using Rebus.Persistence.InMem;
using Rebus.Retry.Simple;
using Rebus.Transport.InMem;

namespace FootballTvPlanner.Server.Common.Messaging;

public static class MessagingExtensions
{
    private const string InputQueueName = "football-tv-planner";

    public static IHostApplicationBuilder AddMessaging(
        this IHostApplicationBuilder builder,
        Assembly handlersAssembly
    )
    {
        builder.Services.AddRebus(configure =>
            configure
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), InputQueueName))
                .Timeouts(t => t.StoreInMemory())
                .Options(o =>
                {
                    o.RetryStrategy(maxDeliveryAttempts: 1, secondLevelRetriesEnabled: true);
                    o.SetWorkerShutdownTimeout(TimeSpan.FromSeconds(20));
                    o.EnableDiagnosticSources();
                })
        );

        builder.Services.AutoRegisterHandlersFromAssembly(handlersAssembly);

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddRebusInstrumentation())
            .WithMetrics(metrics => metrics.AddRebusInstrumentation());

        return builder;
    }
}
