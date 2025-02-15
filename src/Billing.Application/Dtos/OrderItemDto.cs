﻿using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Dtos;

[ExcludeFromCodeCoverage]
public sealed record class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}