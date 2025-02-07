using Billing.Domain.Entities;
using Refit;
using ResultNet;

namespace Billing.Service.Abstractions;

public interface IOderApi
{
    [Get("/{id}")]
    Task<ApiResponse<Result<Order>>> GetOrderAsync(Guid id);
}
