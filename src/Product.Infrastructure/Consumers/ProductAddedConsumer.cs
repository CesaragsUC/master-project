using MassTransit;
using Messaging.Contracts.Events.Product;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using IMediator = MediatR.IMediator;

namespace Product.Infrastructure.Consumers;

[ExcludeFromCodeCoverage]
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
            Log.Information("Startings consume product created message");

            var produto = new ProductAddedEvent
            {
                ProductId = context.Message.ProductId,
                Name = context.Message.Name,
                Active = context.Message.Active,
                Price = context.Message.Price,
                CreateAt = DateTime.Now,
                ImageUri = context.Message.ImageUri
            };

            await _mediator.Send(produto);

            Log.Information("Product message consumed");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao adicionar produto");
        }

        await Task.CompletedTask;
    }
}