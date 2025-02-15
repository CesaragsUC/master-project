using Billing.Application.Dtos;
using Billing.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Extentions;

[ExcludeFromCodeCoverage]
public static class OrderExtentions
{
    public static Order ToOrder(this OrderDto dto)
    {
        return new Order
        {
            Id = dto.Id,
            CreatedAt = dto.CreatedAt,
            CustomerId = dto.CustomerId,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status
        };
    }
}