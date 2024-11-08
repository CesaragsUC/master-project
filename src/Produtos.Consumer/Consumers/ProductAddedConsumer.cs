using Domain.Events;
using MassTransit;
using Serilog;

using IMediator = MediatR.IMediator;

namespace Produtos.Consumer.Consumers;


public class ProductAddedConsumer : IConsumer<ProductAddedEvent>
{
    private readonly IMediator _mediator;

    public ProductAddedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ProductAddedEvent> context)
    {
        try
        {
            var produto = new ProductAddedEvent
            {
                ProductId = context.Message.ProductId,
                Name = context.Message.Name,
                Active = context.Message.Active,
                Price = context.Message.Price,
                CreatAt = DateTime.Now,
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