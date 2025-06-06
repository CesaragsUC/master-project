﻿using MediatR;
using Message.Broker.Abstractions;
using Messaging.Contracts.Events.Orders;
using Order.Domain.Abstraction;
using Order.Order.Application.Abstractions;
using Serilog;
using Shared.Kernel.Core.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class OrderUpdateEventHandler :
    IRequestHandler<OrderUpdateddEvent, bool>
{
    private readonly IOrderRepository _unitOfWork;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IQueueService _queueService;

    public OrderUpdateEventHandler(IOrderRepository unitOfWork,
        IRabbitMqService rabbitMqService,
        IQueueService queueService)
    {
        _unitOfWork = unitOfWork;
        _rabbitMqService = rabbitMqService;
        _queueService = queueService;
    }

    public async Task<bool> Handle(OrderUpdateddEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _unitOfWork.FindAsync(x => x.Id == request.OrderId);

            if (order is null)
            {
                Log.Error("Order not found: {Id}", request.OrderId);
                return false;
            }

            order.Status = request.Status;

            _unitOfWork.Update(order);
            await _unitOfWork.Commit();

            Log.Information("OrderId: {Id} - updated status to: {Status} - At: {Date}", order.Id, request.Status, DateTime.Now);

            if(request.Status == (int)PaymentStatus.Completed)
            {
                var deleteCartEvent = new DeleteCartEvent
                {
                    CustomerId = order.CustomerId
                };

                await _rabbitMqService.Send(deleteCartEvent, _queueService.DeleteCartMessage);
            }


            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }
}
