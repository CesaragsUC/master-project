using AutoMapper;
using Messaging.Contracts.Events.Product;
using Product.Application.MongoEntities;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Configurations;

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
