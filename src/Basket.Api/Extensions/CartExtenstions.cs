using Basket.Api.Dtos;
using Basket.Domain.Entities;
using Messaging.Contracts.Events.Checkout;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class CartExtenstions
{
    public static Cart ToCart(this CartDto cartDto)
    {
        var cart = new Cart
        {
            CustomerId = cartDto.CustomerId,
            Items = cartDto.Items.Select(item => item.ToCartItem()).ToList(),
            TotalPrice = TotalPrice(cartDto.Items)
        };

        return cart;
    }

    public static CartItem ToCartItem(this CartItensDto cartItensDto)
    {
        var cartItem = new CartItem
        {
            ProductId = cartItensDto.ProductId,
            ProductName = cartItensDto.ProductName,
            Quantity = cartItensDto.Quantity,
            UnitPrice = cartItensDto.UnitPrice,
            TotalPrice = cartItensDto.TotalPrice,
            ImageUrl = cartItensDto.ImageUrl
        };

        return cartItem;
    }

    public static CartItensDto  ToCartItemDto(this CartItem cartItensDto)
    {
        var cartItem = new CartItensDto
        {
            ProductId = cartItensDto.ProductId,
            ProductName = cartItensDto.ProductName,
            Quantity = cartItensDto.Quantity,
            UnitPrice = cartItensDto.UnitPrice,
            ImageUrl = cartItensDto.ImageUrl
        };

        return cartItem;
    }

    public static CheckoutEvent ToCheckoutEvent(this CartDto cartDto)
    {
        var checkoutEvent = new CheckoutEvent
        {
            CustomerId = cartDto.CustomerId,
            UserName = cartDto.UserName,
            Status = (int)OrderStatus.Created,
            Items = cartDto.Items.Select(item => new CartItensDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ImageUrl = item.ImageUrl
            }).ToList(),

            TotalPrice = cartDto.TotalPrice,
            PaymentToken = PaymentTokenService.GenerateToken()
        };
        return checkoutEvent;

    }

    public static decimal TotalPrice(this List<CartItensDto> itens)
    {
        return itens.Sum(item => item.TotalPrice);
    }

    public static CartDto ToDto(this Cart cart)
    {
        var cartDto = new CartDto
        {
            CustomerId = cart.CustomerId,
            Items = cart.Items.Select(item => item.ToCartItemDto()).ToList(),
            TotalPrice = cart.TotalPrice
        };
        return cartDto;
    }
}
