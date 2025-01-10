using AutoMapper;
using Basket.Api.Abstractions;
using Basket.Api.Dtos;
using Basket.Api.Events;
using Basket.Domain.Abstractions;
using Basket.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using ResultNet;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Services;

[ExcludeFromCodeCoverage]
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishedMessage;
    private readonly IDiscountApi _discountApi;

    public CartService(ICartRepository cartRepository,
        ICacheService cacheService,
        IMapper mapper,
        IPublishEndpoint publishedMessage,
        IDiscountApi discountApi)
    {
        _cartRepository = cartRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _publishedMessage = publishedMessage;
        _discountApi = discountApi;
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
        var cart = _mapper.Map<Cart>(cartDto);

        var cacheKey = $"cart:{cart.CustomerId}";

        var result = await _cacheService.GetOrCreateAsync(
            key: cacheKey,
            factory: async () =>
            {
                // Retorna o estado mais recente do carrinho (pode ser o próprio parâmetro)
                return cart;
            },
             Expiration,
            updateDatabase: async (updatedCart) =>
            {
                await _cartRepository.UpsertAsync(cart);
            });

        if (result is not null)
        {
            return await Result<bool>.SuccessAsync("product saved");
        }
        else
        {
            return await Result<bool>.FailureAsync("An error occour while attempt to save product.");
        }
    }

    public async Task<Result<bool>> CheckoutAsync(CartCheckoutDto checkoutDto)
    {
        try
        {
            var checkout = _mapper.Map<BasketCheckoutEvent>(checkoutDto);

            await _publishedMessage.Publish<BasketCheckoutEvent>(checkout);

            return await Result<bool>.SuccessAsync(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error to send checkout to Queue");
            throw;
        }

    }

    public async Task<Result<DiscountResponse>> ApplyDiscountAsync(DiscountRequest discountRequest)
    {
        var response = await _discountApi.ApplyDiscountAsync(discountRequest);

        if(response.IsSuccessStatusCode)
        {
            //return await Result<DiscountResponse>.SuccessAsync(new DiscountResponse
            //{
            //    Code = response?.Content?.Code,
            //    Type = response.Content.Type,
            //    Value = response.Content.Value,
            //    MinValue = response.Content.MinValue,
            //});

            //mock
            return await Result<DiscountResponse>.SuccessAsync(new DiscountResponse
            {
                Code = "CASOFT20",
                Type = 1,
                Value = 20,
                MinValue = 100,
            });
        }
        else
        {
            return await Result<DiscountResponse>.FailureAsync("An error occour while attempt to apply discount.");
        }

    }

}
