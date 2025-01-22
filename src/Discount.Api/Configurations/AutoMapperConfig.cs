using AutoMapper;
using Discount.Domain.Dtos;
using Discount.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Discount.Api.Configurations;

[ExcludeFromCodeCoverage]
public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Coupon, CouponCreateDto>().ReverseMap();
        CreateMap<Coupon, CouponUpdateDto>().ReverseMap();
    }
}
