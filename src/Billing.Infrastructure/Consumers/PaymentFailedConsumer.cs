using MassTransit;
using MediatR;
using Messaging.Contracts.Events.Payments;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Infrastructure.Consumers;

[ExcludeFromCodeCoverage]
public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly IMediator _mediator;
    public PaymentFailedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {

        await _mediator.Send(new PaymentFailedEvent
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
