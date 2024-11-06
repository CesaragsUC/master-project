using Domain;
using Domain.Interfaces;
using Infrasctructure;
using Infrastructure;
using Infrastructure.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    builder.Services.AddMassTransit(m =>
    {
        m.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host("localhost", "/", c =>
            {
                c.Username("guest");
                c.Password("guest");
            });
            cfg.ConfigureEndpoints(ctx);
        });
    });

    builder.Services.AddMediatrService();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    builder.Services.PostgresDbService(builder.Configuration);

    ////op��o 1 com classe de configura��o
    //builder.Services.AddScoped<IMigratorService, MigratorService>();

    ////op��o 2 com extens�o
    builder.Services.ConfigureFluentMigration(builder.Configuration);


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao iniciar a aplica��o");
    throw;
}


