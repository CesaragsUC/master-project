using Basket.Domain.Abstractions;
using MassTransit;
using Messaging.Contracts.Events.Orders;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Infrastructure.Service;

[ExcludeFromCodeCoverage]
public class RemoveCartConsumer : IConsumer<DeleteCartEvent>
{
    private readonly ICartRepository _cartRepository;
    public RemoveCartConsumer(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task Consume(ConsumeContext<DeleteCartEvent> context)
    {
        try
        {
           var cart = await _cartRepository.GetAsync(context.Message.CustomerId);

            if (cart is  not null)
            {
                await _cartRepository.DeleteAsync(cart.CustomerId);

                Log.Information($"Cart deleted for customerid {context.Message.CustomerId}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error while try delete cart for customerid {context.Message.CustomerId}");
            throw;
        }

        await Task.CompletedTask;
    }
}
