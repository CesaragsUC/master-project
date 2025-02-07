using Application.Dtos.Dtos.Payments;
using AutoMapper;
using Billing.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Billing.Api.Configurations;

[ExcludeFromCodeCoverage]
public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<Payment,PaymentDto>().ReverseMap();
        CreateMap<Payment, PaymentCreatDto>().ReverseMap();
    }
}
