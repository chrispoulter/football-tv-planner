using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballTvPlanner.Server.Common.Infrastructure;

public static class DataProtectionExtensions
{
    public static IHostApplicationBuilder AddDataProtection<TContext>(
        this IHostApplicationBuilder builder
    )
        where TContext : DbContext, IDataProtectionKeyContext
    {
        var dataProtection = builder
            .Services.AddDataProtection()
            .SetApplicationName(builder.Environment.ApplicationName)
            .PersistKeysToDbContext<TContext>();

        var certificate = builder.Configuration["DataProtection:Certificate"];

        if (!string.IsNullOrEmpty(certificate))
        {
            dataProtection.ProtectKeysWithCertificate(
                X509CertificateLoader.LoadPkcs12(Convert.FromBase64String(certificate), null)
            );
        }

        return builder;
    }
}
