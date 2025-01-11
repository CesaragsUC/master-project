﻿using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Dtos;

[ExcludeFromCodeCoverage]
public class DiscountResponse
{
    public decimal TotalDiscount { get; set; }
}
