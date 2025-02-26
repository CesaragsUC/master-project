﻿using Basket.Api.Abstractions;
using Basket.Api.Dtos;
using Basket.Api.Extensions;
using Basket.Domain.Abstractions;
using Basket.Domain.Entities;
using Basket.Infrastructure.RabbitMq;
using Message.Broker.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using ResultNet;
using Serilog;
using Shared.Kernel.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Basket.Api.Services;

[ExcludeFromCodeCoverage]
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICacheService _cacheService;
    private readonly IDiscountApi _discountApi;
    private readonly IQueueService _queueService;
    private readonly IRabbitMqService _rabbitMqService;

    public CartService(ICartRepository cartRepository,
        ICacheService cacheService,
        IDiscountApi discountApi,
        IQueueService queueService,
        IRabbitMqService rabbitMqService)
    {
        _cartRepository = cartRepository;
        _cacheService = cacheService;
        _discountApi = discountApi;
        _queueService = queueService;
        _rabbitMqService = rabbitMqService;
    }


    [ExcludeFromCodeCoverage]
    private DistributedCacheEntryOptions Expiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),

        //should always be set lower than the absolute expiration
        SlidingExpiration = TimeSpan.FromMinutes(1)
    };


    public async Task<Result<Cart?>> GetCartAsync(Guid customerId)
    {
        try
        {
            var cacheKey = $"cart:{customerId}";

            var cart = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var cartFromDb = await _cartRepository.GetAsync(customerId);

                return cartFromDb;

            }, Expiration);

            if (cart is not null)
            {
                return await Result<Cart?>.SuccessAsync(cart);
            }
            else
            {
                return await Result<Cart?>.FailureAsync("product not found");
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while fetching cart from Redis Cache");
            throw;
        }
    }

    public async Task<Result<bool>> SaveOrUpdateCartAsync(CartDto cartDto)
    {
        var cart = cartDto.ToCart();

        var cacheKey = $"cart:{cart.CustomerId}";

        var result = await _cacheService.GetOrCreateAsync(
            key: cacheKey,
            factory: async () =>
            {
                return cart;
            },
             Expiration,
            updateDatabase: async (updatedCart) =>
            {
                await _cartRepository.UpsertAsync(cart);
            });

        if (result is not null)
        {
            return await Result<bool>.SuccessAsync("cart saved");
        }
        else
        {
            return await Result<bool>.
                FailureAsync("An error occour while attempt to save product.");
        }
    }

    public async Task<Result<bool>> CheckoutAsync(CartDto checkoutDto)
    {
        try
        { 
            var orderCreated = checkoutDto.ToCheckoutEvent();
            orderCreated.PaymentToken = PaymentTokenService.GenerateToken();

            await _rabbitMqService.Send(orderCreated, _queueService.OrderCheckoutEventMessage);

            return await Result<bool>.SuccessAsync("order sent to queue");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send checkout to Queue");
            throw;
        }

    }

    public async Task<Result<CartDto>> ApplyDiscountAsync(CartDto cartDto)
    {

        var response = await _discountApi.
                            ApplyDiscountAsync(cartDto.CouponCode!,
                            cartDto.TotalPrice);

       

        if (response.IsSuccessStatusCode && response.Content!.Succeed)
        {
            cartDto.SetDiscount(response.Content!.TotalDiscount);
            return await Result<CartDto>.SuccessAsync(cartDto);
        }
        else
        {
            var error = JsonSerializer.Deserialize<DiscountResponse>(response.Error.Content);
            return await Result<CartDto>.FailureAsync(error.Message);
        }

    }

}
