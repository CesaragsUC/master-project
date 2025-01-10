using AutoMapper;
using Basket.Api.Dtos;
using Basket.Api.Events;
using Basket.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Basket.Api.Configurations;

[ExcludeFromCodeCoverage]
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<Cart, CartDto>().ReverseMap();
        CreateMap<CartItem, CartItensDto>().ReverseMap();
        CreateMap<CartCheckoutDto, BasketCheckoutEvent>().ReverseMap();
    }
}
