using MediatR;
using Product.Application.Queries.Product;
using Product.Domain.Exceptions;
using RepoPgNet;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Product;

[ExcludeFromCodeCoverage]
public class ProdutoQueryHandler : 
    IRequestHandler<ProductQuery, List<Domain.Models.Product>>,
    IRequestHandler<ProductByIdQuery, Domain.Models.Product>
{

    private readonly IPgRepository<Domain.Models.Product> _repository;

    public ProdutoQueryHandler(IPgRepository<Domain.Models.Product> repository)
    {
        _repository = repository;
    }

    public async Task<List<Domain.Models.Product?>> Handle(ProductQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetAll().ToList()!;
    }

    public async Task<Domain.Models.Product?> Handle(ProductByIdQuery request, CancellationToken cancellationToken)
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
