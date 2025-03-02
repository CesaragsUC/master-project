using Billing.Application.Dtos;
using Refit;
using ResultNet;

namespace Billing.Application.Abstractions;

public interface IOderApi
{
    [Get("/{orderId}/{customerId}")]
    Task<ApiResponse<Result<OrderDto>>> GetOrderAsync(Guid orderId,Guid customerId);
}
