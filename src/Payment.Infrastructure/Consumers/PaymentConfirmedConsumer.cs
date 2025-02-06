using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Payments;

namespace Billing.Infrastructure.Consumers;

public class PaymentConfirmedConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly IMediator _mediator;
    public PaymentConfirmedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        await _mediator.Send(new PaymentConfirmedEvent
        {
            PaymentToken = context.Message.PaymentToken,
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount,
            PaymentDate = context.Message.PaymentDate,
            PaymentMethod = context.Message.PaymentMethod,
            Status = context.Message.Status
        });
    }
}