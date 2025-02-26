﻿using MassTransit;
using Messaging.Contracts.Events.Product;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using IMediator = MediatR.IMediator;

namespace Product.Infrastructure.Consumers;

[ExcludeFromCodeCoverage]
public class ProdutctDeletedConsumer : IConsumer<ProductDeletedEvent>
{
    private readonly IMediator _mediator;

    public ProdutctDeletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        try
        {
            var produto = new ProductDeletedEvent
            {
                ProductId = context.Message.ProductId
            };

            await _mediator.Send(produto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to delete product");
        }

        await Task.CompletedTask;
    }
}