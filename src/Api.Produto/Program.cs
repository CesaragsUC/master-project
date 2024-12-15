using Product.Api.Configuration;
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

    builder.Services.AddServices(builder.Configuration);


    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao iniciar a aplicação");
    throw;
}


