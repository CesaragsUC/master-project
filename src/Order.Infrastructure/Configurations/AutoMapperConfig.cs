﻿using AutoMapper;
using Order.Service.Dto;

namespace Order.Infrastructure.Configurations;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<OrderDto, Domain.Entities.Order>().ReverseMap();
        CreateMap<CreateOrderDto, Domain.Entities.Order>().ReverseMap();
        CreateMap<OrderItemDto, Domain.Entities.OrderItem>().ReverseMap();

    }
}
