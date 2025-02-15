using Billing.Application.Dtos;
using Refit;
using ResultNet;

namespace Billing.Application.Abstractions;

public interface IOderApi
{
    [Get("/{id}")]
    Task<ApiResponse<Result<OrderDto>>> GetOrderAsync(Guid id);
}
