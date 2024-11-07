using Domain.Events;
using MassTransit;
using Serilog;

using IMediator = MediatR.IMediator;

namespace Produtos.Consumer.Consumers;


public class ProdutoAtualizadoConsumer : IConsumer<ProdutoAtualizadoEvent>
{
    private readonly IMediator _mediator;

    public ProdutoAtualizadoConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ProdutoAtualizadoEvent> context)
    {
        try
        {
            var produto = new ProdutoAtualizadoEvent
            {
                ProdutoId = context.Message.ProdutoId,
                Nome = context.Message.Nome,
                Active = context.Message.Active,
                Preco = context.Message.Preco,
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