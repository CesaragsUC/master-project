using Microsoft.Extensions.Hosting;
using Produtos.Consumer.Configurations;
using Serilog;


class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        var host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Registra os serviços da aplicação.
                services.AddServices(context.Configuration);
            });
}