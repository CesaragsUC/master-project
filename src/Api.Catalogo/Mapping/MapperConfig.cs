using Api.Catalogo.Dtos;
using Api.Catalogo.Models;
using AutoMapper;

namespace Api.Catalogo.Mapping
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ProductCreateDto, Product>().ReverseMap();
            CreateMap<ProductUpdateDto, Product>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
