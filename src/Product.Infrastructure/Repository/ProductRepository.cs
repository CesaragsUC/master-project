using HybridRepoNet.Abstractions;
using Infrastructure;
using Product.Domain.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Product.Infrastructure.Repository;

[ExcludeFromCodeCoverage]
public class ProductRepository : IProductRepository
{
    public readonly IUnitOfWork<ProductDbContext> _unitOfWork;

    public ProductRepository(IUnitOfWork<ProductDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(Domain.Models.Product produto)
    {
        await _unitOfWork.Repository<Domain.Models.Product>().AddAsync(produto);
    }

    public void Delete(Domain.Models.Product produto)
    {
        _unitOfWork.Repository<Domain.Models.Product>().Delete(produto);
    }

    public Domain.Models.Product? FindOne(Guid id)
    {
       return  _unitOfWork.Repository<Domain.Models.Product>().FindOne(x => x.Id == id);
    }

    public async Task<IEnumerable<Domain.Models.Product>> GetAllAsync()
    {
        return await _unitOfWork.Repository<Domain.Models.Product>().GetAllAsync();
    }

    public void Update(Domain.Models.Product produto)
    {

        _unitOfWork.Repository<Domain.Models.Product>().Update(produto);
    }

    public Task<bool> Commit()
    {
        return _unitOfWork.Commit();
    }
}
