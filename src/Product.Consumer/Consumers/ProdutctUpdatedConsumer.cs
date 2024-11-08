using MassTransit;
using Messaging.Contracts.Events.Product;
using Serilog;

using IMediator = MediatR.IMediator;

namespace Product.Consumer.Consumers.Configurations;


public class ProdutctUpdatedConsumer : IConsumer<ProductUpdatedEvent>
{
    private readonly IMediator _mediator;

    public ProdutctUpdatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        try
        {
            var produto = new ProductUpdatedEvent
            {
                ProductId = context.Message.ProductId,
                Name = context.Message.Name,
                Active = context.Message.Active,
                Price = context.Message.Price,
                ImageUri = context.Message.ImageUri
            };

            await _mediator.Send(produto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao adicionar produto");
        }

        await Task.CompletedTask;
    }
}