using System.Reflection;
using FluentEmail.Core;

namespace SoccerTv.Api.Common.Email;

public class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task SendTemplateEmailAsync(
        string toAddress,
        string subject,
        string template,
        object model,
        CancellationToken cancellationToken = default
    )
    {
        var sendResponse = await fluentEmail
            .To(toAddress)
            .Subject(subject)
            .UsingTemplateFromEmbedded(template, model, Assembly.GetExecutingAssembly())
            .SendAsync(cancellationToken);

        if (!sendResponse.Successful)
        {
            var errorMessage = string.Join("; ", sendResponse.ErrorMessages);

            throw new Exception($"Failed to send email with template {template}: {errorMessage}");
        }
    }
}
