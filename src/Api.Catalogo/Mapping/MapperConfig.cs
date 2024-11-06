using Api.Catalogo.Dtos;
using Api.Catalogo.Models;
using AutoMapper;

namespace Api.Catalogo.Mapping
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ProdutoAddDto, Produtos>().ReverseMap();
            CreateMap<ProdutoUpdateDto, Produtos>().ReverseMap();
            CreateMap<Produtos, ProdutoDto>().ReverseMap();
        }
    }
}
