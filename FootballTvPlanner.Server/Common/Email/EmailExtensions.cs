using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FootballTvPlanner.Server.Common.Email;

public static class EmailExtensions
{
    public static IHostApplicationBuilder AddEmailServices(
        this IHostApplicationBuilder builder,
        string connectionName
    )
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionName);

        builder
            .Services.AddOptions<EmailSettings>()
            .Bind(builder.Configuration.GetRequiredSection(EmailSettings.SectionName))
            .PostConfigure(settings =>
            {
                if (connectionString is not null)
                {
                    settings.ParseConnectionString(connectionString);
                }
            });

        builder.Services.AddSingleton<EmailTemplateRenderer>();
        builder.Services.AddScoped<IEmailService, EmailService>();

        MailKit.Telemetry.SmtpClient.Configure();

        builder
            .Services.AddOpenTelemetry()
            .WithTracing(tracing =>
                tracing.AddSource(MailKit.Telemetry.SmtpClient.ActivitySourceName)
            )
            .WithMetrics(metrics => metrics.AddMeter(MailKit.Telemetry.SmtpClient.MeterName));

        return builder;
    }
}
