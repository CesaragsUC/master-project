using Order.Application.Dto;
using ResultNet;

namespace Order.Application.Abstractions;

public interface IOrderService
{
    Task<Result<bool>> Add(CreateOrderDto orderDto);
    Task<Result<bool>> Update(UpdateOrderDto orderDto);
    Task<Result<bool>> Delete(Guid id);
    Task<Result<IEnumerable<OrderDto>>> Get(Guid custumerId);
    Task<Result<OrderDto>> Get(Guid orderId, Guid customerId);
    Task<Result<IEnumerable<OrderDto>>> List();
}