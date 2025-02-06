using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Checkout;
using Messaging.Contracts.Events.Orders;
using Serilog;

namespace Order.Infrastructure.Consumers;


public class OrderConsumer : IConsumer<CheckoutEvent>
{
    private readonly IMediator _mediator;

    public OrderConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CheckoutEvent> context)
    {
        try
        {
            var checkout = new OrderCreatedEvent
            {
                CustomerId = context.Message.CustomerId,
                UserName = context.Message.UserName,
                Email = context.Message.Email,
                Status = context.Message.Status,
                PaymentToken = context.Message.PaymentToken,
                Items = context.Message.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                Total = context.Message.TotalPrice
            };

            await _mediator.Send(checkout);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao adicionar produto");
        }

        await Task.CompletedTask;
    }
}
