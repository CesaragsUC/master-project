using HybridRepoNet.Abstractions;
using MediatR;
using Messaging.Contracts.Events.Orders;
using Order.Application.Extentions;
using Order.Domain.Entities;
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
            Domain.Entities.Order? order;

            order = await GetOrderIfExist(request);

            if (order is not null)
            {
                // delete existent items
                await DeleteExistentItems(order, request);

                order.Items = request.Items!.Select(x => x.ToOrderItem(order.Id)).ToList();
                order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);

                // update order items
                await _unitOfWork.Repository<OrderItem>().AddAsync(order.Items!);
                await _unitOfWork.Commit();

                //update total amount
                _unitOfWork.Repository<Domain.Entities.Order>().Update(order);
                await _unitOfWork.Commit();

                Log.Information("Order items updated: {Id} - {Date}", order.Id, DateTime.Now);
            }
            else
            {
                order = request.ToOrder();

                await _unitOfWork.Repository<Domain.Entities.Order>().AddAsync(order);
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
        var order = await _unitOfWork.Repository<Domain.Entities.Order>().FindAsync(
            x => x.CustomerId == request.CustomerId
            && x.Status == (int)OrderStatus.Created,
             i => i.Items!);
        return order;
    }

    private async Task DeleteExistentItems (Domain.Entities.Order order, OrderCreatedEvent request)
    {
        _unitOfWork.Repository<OrderItem>().Delete(x => x.OrderId == order.Id);
        await _unitOfWork.Commit();
    }
}
