using Application.Dtos.Dtos.Produtos;
using AutoMapper;
using Catalog.Domain.Models;

namespace Api.Catalogo.Mapping
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ProductCreateDto, Products>().ReverseMap();
            CreateMap<ProductUpdateDto, Products>().ReverseMap();
            CreateMap<Products, ProductDto>().ReverseMap();
        }
    }
}
