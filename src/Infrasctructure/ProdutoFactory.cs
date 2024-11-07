using Bogus;
using Domain.Models;

namespace Infrasctructure
{
    public static class ProdutoFactory
    {
        public static Produtos CreateProduto()
        {
            Faker bogus = new Faker();
            return new Produtos
            {
                Id = bogus.Random.Guid(),
                Nome = bogus.Commerce.ProductName(),
                Preco = bogus.Random.Decimal(1, 1000),
                Active = bogus.Random.Bool(),

            };

        }

        public static Produtos CreateNullProduto()
        {
            return null;
        }

        public static List<Produtos> CreateProdutoList(int total)
        {
            var produtos = new List<Produtos>();
            for (int i = 0; i < total; i++)
            {
                produtos.Add(CreateProduto());
            }

            Faker bogus = new Faker();
            var produto = new Produtos
            {
                Id = Guid.Parse("dfa9057b-9d9e-427f-9e94-4fbd0d3d02a2"),
                Nome = bogus.Commerce.ProductName(),
                Preco = bogus.Random.Decimal(1, 1000),
                Active = bogus.Random.Bool(),

            };

            produtos.Add(produto);

            return produtos;

        }
    }
}
