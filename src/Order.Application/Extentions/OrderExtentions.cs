using Messaging.Contracts.Events.Orders;
using Order.Domain.Entities;

namespace Order.Application.Extentions;

public static class OrderExtentions
{
    public static Domain.Entities.Order ToOrder(this OrderCreatedEvent orderCreatedEvent)
    {
        var order = new  Domain.Entities.Order
        {
            CreatedAt = DateTime.Now,
            CustomerId = orderCreatedEvent.CustomerId,
            Status = (int)OrderStatus.Created,
            Name = orderCreatedEvent.UserName,
            PaymentToken = orderCreatedEvent.PaymentToken

        };

        order.Items = orderCreatedEvent?.Items?.Select(x => x.ToOrderItem(order.Id)).ToList();
        order.TotalAmount = order!.Items!.Sum(x => x.UnitPrice * x.Quantity);

        return order;
    }

    public static OrderItem ToOrderItem(this OrderItemDto orderItemDto, Guid orderId)
    {
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = orderItemDto.ProductId,
            Quantity = orderItemDto.Quantity,
            UnitPrice = orderItemDto.UnitPrice,
            ProductName = orderItemDto.ProductName,
            TotalPrice = orderItemDto.Quantity * orderItemDto.UnitPrice

        };

    }
}
