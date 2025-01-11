using AutoMapper;
using Discount.Domain.Dtos;
using Discount.Domain.Entities;

namespace Discount.Api.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Coupon, CouponCreateDto>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDto>().ReverseMap();
        }
    }
}
