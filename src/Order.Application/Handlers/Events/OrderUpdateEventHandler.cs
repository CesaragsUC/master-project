using HybridRepoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Orders;
using Order.Application.Extentions;
using Order.Infrastructure;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class OrderUpdateEventHandler :
    IRequestHandler<OrderUpdateddEvent, bool>
{
    private readonly IUnitOfWork<OrderDbContext> _unitOfWork;

    public OrderUpdateEventHandler(IUnitOfWork<OrderDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(OrderUpdateddEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _unitOfWork.Repository<Domain.Entities.Order>().FindAsync(x => x.Id == request.OrderId);

            if (order is null)
            {
                Log.Error("Order not found: {Id}", request.OrderId);
                return false;
            }

            order.Status = request.Status;

            _unitOfWork.Repository<Domain.Entities.Order>().Update(order);
            await _unitOfWork.Commit();

            Log.Information("Order updated: {Id} - {Date}", order.Id, DateTime.Now);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }
}
