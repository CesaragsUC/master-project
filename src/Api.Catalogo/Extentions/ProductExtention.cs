using Application.Dtos.Dtos.Produtos;
using Catalog.Domain.Models;

namespace Catalog.Api.Extentions
{
    public static class ProductExtention
    {

        public static ProductDto ToProductDto(Products produto)
        {
            return new ProductDto
            {
                ProductId = produto.ProductId,
                Name = produto.Name,
                Price = produto.Price,
                ImageUri = produto.ImageUri,
                Active = produto.Active,
                CreatAt = produto.CreatedAt
            };
        }

        public static ProductCreateDto ToProductCreateDto(Products produto)
        {
            return new ProductCreateDto
            {
                Name = produto.Name,
                Price = produto.Price,
                Active = produto.Active
            };
        }

        public static  ProductUpdateDto ToProductUpdateDto(Products produto)
        {
            return new ProductUpdateDto
            {
                ProductId = produto.ProductId,
                Name = produto.Name,
                Price = produto.Price,
                Active = produto.Active,
            };
        }
    }
}
