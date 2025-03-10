﻿using AutoMapper;
using Order.Application.Dto;
using System.Diagnostics.CodeAnalysis;

namespace Order.Api.Configurations;

[ExcludeFromCodeCoverage]
public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<OrderDto, Domain.Entities.Order>().ReverseMap();
        CreateMap<CreateOrderDto, Domain.Entities.Order>().ReverseMap();
        CreateMap<OrderItemDto, Domain.Entities.OrderItem>().ReverseMap();
    }
}
