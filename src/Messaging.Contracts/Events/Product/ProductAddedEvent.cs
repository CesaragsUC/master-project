﻿using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Product;

[ExcludeFromCodeCoverage]
public class ProductAddedEvent : IRequest<bool>
{
    public string? ProductId { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public bool Active { get; set; }

    public string? ImageUri { get; set; }

    public DateTime? CreateAt { get; set; }
        
}
