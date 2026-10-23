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

        return builder;
    }
}
