using Basket.Api.Dtos;
using Basket.Domain.Entities;
using Messaging.Contracts.Events.Checkout;
using Shared.Kernel.Core.Enuns;
using Shared.Kernel.Utils;

namespace Basket.Api.Extensions;

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
            TotalPrice = cartItensDto.TotalPrice
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
                UnitPrice = item.UnitPrice
            }).ToList(),
            TotalPrice = cartDto.TotalPrice,
            PaymentToken = PaymentTokenService.GenerateToken()
        };
        return checkoutEvent;

    }

    private static decimal TotalPrice(this List<CartItensDto> itens)
    {
        return itens.Sum(item => item.TotalPrice);
    }
}
