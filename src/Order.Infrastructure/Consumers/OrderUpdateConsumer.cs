using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Orders;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Order.Infrastructure.Consumers;

[ExcludeFromCodeCoverage]
public class OrderUpdateConsumer : IConsumer<OrderUpdateddEvent>
{
    private readonly IMediator _mediator;

    public OrderUpdateConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<OrderUpdateddEvent> context)
    {
        try
        {
            var orderEvent = new OrderUpdateddEvent
            {
                CustomerId = context.Message.CustomerId,
                OrderId = context.Message.OrderId,
                Status = context.Message.Status
            };

            await _mediator.Send(orderEvent);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error try send order update event");
        }

        await Task.CompletedTask;
    }
}
