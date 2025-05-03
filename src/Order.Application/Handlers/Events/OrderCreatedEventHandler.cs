using MediatR;
using Messaging.Contracts.Events.Orders;
using Order.Application.Extentions;
using Order.Domain.Abstraction;
using Serilog;
using Shared.Kernel.Core.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class OrderCreatedEventHandler :
    IRequestHandler<OrderCreatedEvent, bool>
{
    private readonly IOrderRepository _unitOfWork;

    public OrderCreatedEventHandler(IOrderRepository unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(OrderCreatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Order? order;

            order = await GetOrderIfExist(request);

            if (order is not null)
            {
                // delete existent items
                await DeleteExistentItems(order);

                order.Items = request.Items!.Select(x => x.ToOrderItem(order.Id)).ToList();
                order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

                // update order items
                await _unitOfWork.AddAsync(order.Items);
                await _unitOfWork.Commit();

                //update total amount
                _unitOfWork.Update(order);
                await _unitOfWork.Commit();

                Log.Information("Order items updated: {Id} - {Date}", order.Id, DateTime.Now);
            }
            else
            {
                order = request.ToOrder();

                await _unitOfWork.AddAsync(order);
                await _unitOfWork.Commit();

                Log.Information("Order created: {Id} - {Date}", order.Id, DateTime.Now);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fail to create a order");
            return false;
        }

    }

    private async Task<Domain.Entities.Order> GetOrderIfExist(OrderCreatedEvent request)
    {
        var order = await _unitOfWork.FindAsync(
            x => x.CustomerId == request.CustomerId
            && x.Status == (int)OrderStatus.Created,
             i => i.Items!);
        return order;
    }

    private async Task DeleteExistentItems(Domain.Entities.Order order)
    {
        var result = _unitOfWork.FindOne(order.Id);
        _unitOfWork.Delete(result!);
        await _unitOfWork.Commit();
    }
}
