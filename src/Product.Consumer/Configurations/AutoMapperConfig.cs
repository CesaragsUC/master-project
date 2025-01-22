using AutoMapper;
using Messaging.Contracts.Events.Product;
using Product.Consumer.Models;
using System.Diagnostics.CodeAnalysis;

namespace Product.Consumer.Configurations;

[ExcludeFromCodeCoverage]
internal class AutoMapperConfig  : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<ProductAddedEvent, Products>();
        CreateMap<ProductUpdatedEvent, Products>();
        CreateMap<ProductDeletedEvent, Products>();
    }
}
