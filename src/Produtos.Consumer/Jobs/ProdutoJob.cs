using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Produtos.Consumer.Jobs;

internal sealed class ProdutoJob : IJob
{
    readonly IServiceScopeFactory _scopeFactory;

    public ProdutoJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }


    // somente uma demo de como criar um job. Isso não faz nada.
    public async Task Execute(IJobExecutionContext context)
    {
        // Log.Information($"Job executado em {DateTime.Now}");

        // await using var scope = _scopeFactory.CreateAsyncScope();

        // var pub01 = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

        //await pub01.SchedulePublish(TimeSpan.FromSeconds(15),
        //     new DemoMessage { Value = "Hello, World", CreatAt = DateTime.Now });

        // var pub02 = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        //envia a msg na fila
        // await pub02.Publish(new ProductMessage { Id = Guid.NewGuid(),Nome= "Mochila" , Preco = 100, Active= true, CreatAt = DateTime.Now });


        await Task.CompletedTask;
    }
}

public class DemoMessage
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public DateTime CreatAt { get; set; }

    public DemoMessage()
    {
        Id = Guid.NewGuid();
    }
}
