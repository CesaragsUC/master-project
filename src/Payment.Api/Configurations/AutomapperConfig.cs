using Application.Dtos.Dtos.Payments;
using AutoMapper;
using Billing.Domain.Entities;

namespace Billing.Api.Configurations;

public class AutomapperConfig : Profile
{

    public AutomapperConfig()
    {
        CreateMap<Payment,PaymentDto>().ReverseMap();
        CreateMap<Payment, PaymentCreatDto>().ReverseMap();
    }
}
