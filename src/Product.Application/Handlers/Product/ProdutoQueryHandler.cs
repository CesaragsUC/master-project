using HybridRepoNet.Abstractions;
using Infrastructure;
using MediatR;
using Product.Application.Queries.Product;
using Product.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Product.Application.Handlers.Product;

[ExcludeFromCodeCoverage]
public class ProdutoQueryHandler : 
    IRequestHandler<ProductQuery, IEnumerable<Domain.Models.Product>>,
    IRequestHandler<ProductByIdQuery, Domain.Models.Product>
{

    private readonly IUnitOfWork<ProductDbContext> _unitOfWork;

    public ProdutoQueryHandler(IUnitOfWork<ProductDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Domain.Models.Product?>> Handle(ProductQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Domain.Models.Product>().GetAllAsync();
    }

    public async Task<Domain.Models.Product?> Handle(ProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ProductNotFoundException("Product Id couldn't empty");
        }

        var produto = _unitOfWork.Repository<Domain.Models.Product>().FindOne(x => x.Id == request.Id);

        if (produto == null) throw new ProductNotFoundException(request.Id); 

        return produto;
    }
}
