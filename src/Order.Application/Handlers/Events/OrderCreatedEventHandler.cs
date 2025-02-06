using MediatR;
using Messaging.Contracts.Events.Orders;
using Order.Application.Extentions;
using RepoPgNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Handlers.Events;

[ExcludeFromCodeCoverage]
public class OrderCreatedEventHandler :
    IRequestHandler<OrderCreatedEvent, bool>
{
    private readonly IPgRepository<Domain.Entities.Order> _repository;

    public OrderCreatedEventHandler(IPgRepository<Domain.Entities.Order> repository)
    {
        _repository = repository;
    }


    public async Task<bool> Handle(OrderCreatedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var order = request.ToOrder();

            await _repository.AddAsync(order);

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
