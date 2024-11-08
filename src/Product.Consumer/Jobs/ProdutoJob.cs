using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;

namespace Product.Consumer.Jobs;

internal sealed class ProdutoJob : IJob
{
    readonly IServiceScopeFactory _scopeFactory;

    public ProdutoJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }


    // Just simple demo.
    public async Task Execute(IJobExecutionContext context)
    {
        Log.Information($"Job exectuted at {DateTime.Now}");

        await using var scope = _scopeFactory.CreateAsyncScope();

        var pub01 = scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

        await pub01.SchedulePublish(TimeSpan.FromSeconds(15),
             new DemoMessage { Price = 200, CreatAt = DateTime.Now });

        var pub02 = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        // send a msg to queue
        await pub02.Publish(new DemoMessage {
            Id = Guid.NewGuid(),
            Name = "Mochila", 
            Price = 100,
            CreatAt = DateTime.Now 
        });


        await Task.CompletedTask;
    }
}

public class DemoMessage
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }

    public string Name { get; set; }
    public DateTime CreatAt { get; set; }

    public DemoMessage()
    {
        Id = Guid.NewGuid();
    }
}
