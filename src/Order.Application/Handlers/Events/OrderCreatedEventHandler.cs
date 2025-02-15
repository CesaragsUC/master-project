using HybridRepoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Orders;
using Order.Application.Extentions;
using Order.Infrastructure;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class OrderCreatedEventHandler :
    IRequestHandler<OrderCreatedEvent, bool>
{
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;

    public OrderCreatedEventHandler(IUnitOfWork<OrderDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(OrderCreatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var order = request.ToOrder();

            await _unitOfWork.Repository<Domain.Entities.Order>().AddAsync(order);
            await _unitOfWork.Commit();

            Log.Information("Order created: {Id} - {Date}", order.Id, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }
}
