using System.Diagnostics.Metrics;

namespace FootballTvPlanner.Server.Common.Email;

public class EmailMetrics
{
    public static readonly string MeterName = typeof(EmailMetrics).Namespace!;

    private readonly Counter<long> sent;

    public EmailMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        sent = meter.CreateCounter<long>(
            "email.sent",
            unit: "{email}",
            description: "Number of emails sent, by template and outcome."
        );
    }

    public void RecordSent(string template, bool success) =>
        sent.Add(
            1,
            new KeyValuePair<string, object?>("email.template", template),
            new KeyValuePair<string, object?>("email.outcome", success ? "success" : "failure")
        );
}
