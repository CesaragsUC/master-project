
﻿using System.Diagnostics.CodeAnalysis;

namespace Basket.Domain.Entities;

[ExcludeFromCodeCoverage]
public class CartItem
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
