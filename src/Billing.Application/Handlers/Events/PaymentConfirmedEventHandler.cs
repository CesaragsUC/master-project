using AutoMapper;
using Billing.Domain.Entities;
using Billing.Infrastructure;
using Billing.Infrastructure.Configurations.RabbitMq;
using HybridRepoNet.Abstractions;
using MediatR;
using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Orders;
using Messaging.Contracts.Events.Payments;
using Serilog;
using Shared.Kernel.Core.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class PaymentConfirmedEventHandler :
    IRequestHandler<PaymentConfirmedEvent, bool>
{

    private readonly IUnitOfWork<BillingContext> _unitOfWork;
    private readonly IQueueService _queueService;
    private readonly IRabbitMqService _rabbitMqService;

    public PaymentConfirmedEventHandler(
        IUnitOfWork<BillingContext> unitOfWork,
        IQueueService queueService,
        IRabbitMqService rabbitMqService)
    {
        _unitOfWork = unitOfWork;
        _queueService = queueService;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<bool> Handle(PaymentConfirmedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _unitOfWork.Repository<Payment>().FindAsync(x => x.OrderId == request.OrderId);

            if (payment is not null)
            {
                payment.Status = (int)PaymentStatus.Completed;

                _unitOfWork.Repository<Payment>().Update(payment);
                await _unitOfWork.Commit();

                var orderUpdateMessage = new OrderUpdateddEvent()
                {
                    OrderId = request.OrderId,
                    CustomerId = request.CustomerId,
                    Status = (int)OrderStatus.Completed
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
