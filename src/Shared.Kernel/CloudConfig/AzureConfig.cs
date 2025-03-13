using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Shared.Kernel.CloudConfig;

[ExcludeFromCodeCoverage]
public static class AzureConfig
{
    public static void AzureKeyVaultConfig(this IServiceCollection service, 
        IHostApplicationBuilder builder,
        IConfiguration configuration,
        string environment)
    {

        if(string.IsNullOrWhiteSpace(environment) ||
            !environment.Equals("Production"))
        {
            return;
        }

        string jsonFile = $"appsettings.{environment}.json";

        builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile(jsonFile, optional: true);

        var keyVaultUrl = configuration.GetSection("AzureConfig:keyVaultUrl").Value;

        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl!),
            new DefaultAzureCredential()
        );

    }
}
