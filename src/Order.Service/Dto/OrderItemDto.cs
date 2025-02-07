﻿using System.Diagnostics.CodeAnalysis;

namespace Order.Service.Dto;

[ExcludeFromCodeCoverage]
public sealed record class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
}