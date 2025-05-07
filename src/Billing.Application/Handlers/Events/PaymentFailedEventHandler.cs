using Billing.Application.Service;
using Billing.Domain.Abstractions;
using Billing.Domain.Entities;
using MediatR;
using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Orders;
using Messaging.Contracts.Events.Payments;
using Serilog;
using Shared.Kernel.Core.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class PaymentFailedEventHandler :
    IRequestHandler<PaymentFailedEvent, bool>
{
    private readonly IPaymentRepository _unitOfWork;
    private readonly IQueueService _queueService;
    private readonly IRabbitMqService _rabbitMqService;

    public PaymentFailedEventHandler(
        IPaymentRepository unitOfWork,
        IQueueService queueService,
        IRabbitMqService rabbitMqService)
    {
        _unitOfWork = unitOfWork;
        _queueService = queueService;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<bool> Handle(PaymentFailedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _unitOfWork.FindAsync(x => x.OrderId == request.OrderId);

            if (payment is not null)
            {
                payment.Status = (int)PaymentStatus.Failed;

                _unitOfWork.Update(payment);
                await _unitOfWork.Commit();

                var orderUpdateMessage = new OrderUpdateddEvent()
                {
                    OrderId = request.OrderId,
                    CustomerId = request.CustomerId,
                    Status = (int)OrderStatus.PaymentFailed
                };

                await _rabbitMqService.Send(orderUpdateMessage, _queueService.OrderUpdateMessage);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while try update payment");
            return false;
        }

    }
}