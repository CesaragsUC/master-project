using AutoMapper;
using Messaging.Contracts.Events.Product;
using Product.Consumer.Models;

namespace Product.Consumer.Configurations;

internal class AutoMapperConfig  : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<ProductAddedEvent, Products>();
        CreateMap<ProductUpdatedEvent, Products>();
        CreateMap<ProductDeletedEvent, Products>();
    }
}
