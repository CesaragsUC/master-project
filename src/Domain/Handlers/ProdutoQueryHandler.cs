using Domain.Handlers.Queries;
using Domain.Interfaces;
using MediatR;
using Product.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Product.Domain.Handlers;

[ExcludeFromCodeCoverage]
public class ProdutoQueryHandler : 
    IRequestHandler<ProductQuery, List<Models.Product>>,
    IRequestHandler<ProductByIdQuery, Models.Product>
{

    private readonly IRepository<Models.Product> _repository;

    public ProdutoQueryHandler(IRepository<Models.Product> repository)
    {
        _repository = repository;
    }

    public async Task<List<Models.Product?>> Handle(ProductQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetAll().ToList()!;
    }

    public async Task<Models.Product?> Handle(ProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ProductNotFoundException("Product Id couldn't empty");
        }

        var produto = _repository.FindOne(x => x.Id == request.Id);

        if (produto == null) throw new ProductNotFoundException(request.Id); 

        return produto;
    }
}
