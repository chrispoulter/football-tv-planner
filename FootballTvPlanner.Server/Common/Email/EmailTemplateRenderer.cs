using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Encodings.Web;
using Fluid;
using Microsoft.Extensions.Options;

namespace FootballTvPlanner.Server.Common.Email;

public class EmailTemplateRenderer(IOptions<EmailSettings> emailSettings)
{
    private static readonly FluidParser Parser = new();

    private static readonly TemplateOptions TemplateOptions = new()
    {
        MemberAccessStrategy = new UnsafeMemberAccessStrategy(),
    };

    private readonly ConcurrentDictionary<string, IFluidTemplate> templates = new();

    public async Task<string> RenderAsync(string template, object model)
    {
        var fluidTemplate = templates.GetOrAdd(template, ParseTemplate);

        var context = new TemplateContext(model, TemplateOptions);
        context.SetValue("SiteUrl", emailSettings.Value.SiteUrl);

        return await fluidTemplate.RenderAsync(context, HtmlEncoder.Default);
    }

    private static IFluidTemplate ParseTemplate(string template)
    {
        using var stream =
            Assembly.GetEntryAssembly()?.GetManifestResourceStream(template)
            ?? throw new InvalidOperationException($"Email template {template} was not found.");

        using var reader = new StreamReader(stream);

        return Parser.TryParse(reader.ReadToEnd(), out var fluidTemplate, out var error)
            ? fluidTemplate
            : throw new InvalidOperationException(
                $"Email template {template} could not be parsed: {error}"
            );
    }
}
