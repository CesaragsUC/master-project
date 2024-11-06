using Bogus;
using Domain.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Produtos.Consumer;


public class ProdutoConsumer : IConsumer<Produto>
{
    private IServiceScopeFactory _serviceScopeFactory;
    public ProdutoConsumer(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Consume(ConsumeContext<Produto> context)
    {
        Faker faker = new Faker("pt_BR");
        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var _mongoDb = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

            var produto = new ProdutoMessage
            {
                ProdutoId = context.Message.Id.ToString(),
                Nome = context.Message.Nome,
                Active = context.Message.Active,
                Preco = context.Message.Preco,
                CreatAt = faker.Date.Between(new DateTime(2020, 1, 1), DateTime.Now), // Gerar um DateTime aleatório entre duas datas
            };

            await _mongoDb.GetCollection<ProdutoMessage>("Produtos").InsertOneAsync(produto);

            Log.Information($"Adicionado produto nome: {context.Message.Nome} - {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao adicionar produto");
        }

        await Task.CompletedTask;
    }
}