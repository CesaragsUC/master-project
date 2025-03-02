using Messaging.Contracts.Events.Orders;
using Order.Application.Dto;
using Order.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Order.Application.Extentions;

[ExcludeFromCodeCoverage]
public static class OrderExtentions
{
    public static Domain.Entities.Order ToOrder(this OrderCreatedEvent orderCreatedEvent)
    {
        var order = new  Domain.Entities.Order
        {
            CreatedDate = DateTime.Now,
            CustomerId = orderCreatedEvent.CustomerId,
            Status = (int)OrderStatus.Created,
            Name = orderCreatedEvent.UserName,
            PaymentToken = orderCreatedEvent.PaymentToken

        };

        order.Items = orderCreatedEvent?.Items?.Select(x => x.ToOrderItem(order.Id)).ToList();
        order.TotalAmount = order!.Items!.Sum(x => x.UnitPrice * x.Quantity);

        return order;
    }

    public static Domain.Entities.Order ToOrder(this OrderDto orderDto)
    {
        var order = new Domain.Entities.Order
        {
            CreatedDate = DateTime.Now,
            CustomerId = orderDto.CustomerId,
            Status = (int)OrderStatus.Created,
            Name = orderDto.Name,
            PaymentToken = orderDto.PaymentToken

        };

        order.Items = orderDto?.Items?.Select(x => x.ToOrderItem(order.Id)).ToList();
        order.TotalAmount = order!.Items!.Sum(x => x.UnitPrice * x.Quantity);

        return order;
    }

    public static Domain.Entities.Order ToOrder(this CreateOrderDto dto)
    {
        var order = new Domain.Entities.Order
        {
            CreatedDate = DateTime.Now,
            CustomerId = dto.CustomerId,
            Status = (int)OrderStatus.Created,
            Name = dto.Name,
            PaymentToken = dto.PaymentToken

        };

        order.Items = dto?.Items?.Select(x => x.ToOrderItem(order.Id)).ToList();
        order.TotalAmount = order!.Items!.Sum(x => x.UnitPrice * x.Quantity);

        return order;
    }

    public static OrderItem ToOrderItem(this Dto.OrderItemDto orderItemDto, Guid orderId)
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

    public static OrderItem ToOrderItem(this Messaging.Contracts.Events.Orders.OrderItemDto orderItemDto, Guid orderId)
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

    public static Dto.OrderItemDto ToOrderItemDto(this OrderItem orderItem, Guid orderId)
    {
        return new Dto.OrderItemDto
        {
            Id = orderItem.Id,
            OrderId = orderId,
            ProductId = orderItem.ProductId,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            ProductName = orderItem.ProductName,
            TotalPrice = orderItem.Quantity * orderItem.UnitPrice

        };

    }

    public static OrderDto ToOrderDto(this Domain.Entities.Order order)
    {
        var orderDto = new OrderDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedDate,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Name = order.Name,
            PaymentToken = order.PaymentToken
        };

        orderDto.Items = order?.Items?.Select(x => x.ToOrderItemDto(orderDto.Id)).ToList();
        orderDto.TotalAmount = orderDto.Items.Any() ? orderDto.Items.Sum(x => x.UnitPrice * x.Quantity) : 0;

        return orderDto;
    }

   public static IEnumerable<OrderDto> ToOrderListDto(this List<Domain.Entities.Order> orders)
    {
        var orderDto = new List<OrderDto>();

        foreach (var item in orders)
        {
            var dto = new OrderDto
            {
                Id = item.Id,
                CreatedAt = item.CreatedDate,
                CustomerId = item.CustomerId,
                TotalAmount = item.TotalAmount,
                Status = item.Status,
                Name = item.Name,
                PaymentToken = item.PaymentToken
            };

            dto.Items = item?.Items?.Select(x => x.ToOrderItemDto(dto.Id)).ToList();
            dto.TotalAmount = dto.Items.Any() ? dto.Items.Sum(x => x.UnitPrice * x.Quantity) : 0;

            orderDto.Add(dto);

        }

        return orderDto;
    }
}
