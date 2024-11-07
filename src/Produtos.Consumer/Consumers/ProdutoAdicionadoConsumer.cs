using Domain.Events;
using MassTransit;
using Serilog;

using IMediator = MediatR.IMediator;

namespace Produtos.Consumer.Consumers;


public class ProdutoAdicionadoConsumer : IConsumer<ProdutoAdicionadoEvent>
{
    private readonly IMediator _mediator;

    public ProdutoAdicionadoConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ProdutoAdicionadoEvent> context)
    {
        try
        {
            var produto = new ProdutoAdicionadoEvent
            {
                ProdutoId = context.Message.ProdutoId,
                Nome = context.Message.Nome,
                Active = context.Message.Active,
                Preco = context.Message.Preco,
                CreatAt = DateTime.Now,
                ImageUri = context.Message.ImageUri
            };

            await _mediator.Send(produto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao adicionar produto");
        }

        await Task.CompletedTask;
    }
}