using HybridRepoNet.Abstractions;
using Order.Domain.Abstraction;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Order.Infrastructure.Repository;

[ExcludeFromCodeCoverage]
public class OrderRepository : IOrderRepository
{
    public readonly IUnitOfWork<OrderDbContext> _unitOfWork;

    public OrderRepository(IUnitOfWork<OrderDbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(Domain.Entities.Order order)
    {
        await _unitOfWork.Repository<Domain.Entities.Order>().AddAsync(order);
    }

    public async Task AddAsync(IEnumerable<Domain.Entities.OrderItem> orderItem)
    {
        await _unitOfWork.Repository<Domain.Entities.OrderItem>().AddAsync(orderItem);
    }

    public void Delete(Domain.Entities.Order order)
    {
        _unitOfWork.Repository<Domain.Entities.Order>().Delete(order);
    }

    public void SoftDelete(Domain.Entities.Order order)
    {
        _unitOfWork.Repository<Domain.Entities.Order>().Delete(order);
    }

    public Domain.Entities.Order? FindOne(Guid id)
    {
        return _unitOfWork.Repository<Domain.Entities.Order>().FindOne(x => x.Id == id);
    }

    public async Task<Domain.Entities.Order?> FindAsync(Expression<Func<Domain.Entities.Order, bool>> wherePredicate,
        params Expression<Func<Domain.Entities.Order, object>>[] includes)
    {
        return await _unitOfWork.Repository<Domain.Entities.Order>().FindAsync(wherePredicate, includes);
    }

    public async Task<Domain.Entities.Order?> FindAsync(Expression<Func<Domain.Entities.Order, bool>> wherePredicate)
    {
        return await _unitOfWork.Repository<Domain.Entities.Order>().FindAsync(wherePredicate);
    }

    public async Task<IEnumerable<Domain.Entities.Order>> GetAllAsync()
    {
        return await _unitOfWork.Repository<Domain.Entities.Order>().GetAllAsync();
    }

    public async Task<IEnumerable<Domain.Entities.Order?>> GetAllAsync(Expression<Func<Domain.Entities.Order, bool>> wherePredicate,
         params Expression<Func<Domain.Entities.Order, object>>[] includes)
    {
        return await _unitOfWork.Repository<Domain.Entities.Order>().GetAllAsync(wherePredicate, includes);
    }

    public void Update(Domain.Entities.Order order)
    {

        _unitOfWork.Repository<Domain.Entities.Order>().Update(order);
    }

    public Task<bool> Commit()
    {
        return _unitOfWork.Commit();
    }

    public async Task<IEnumerable<Domain.Entities.Order?>> GetAllAsync(
        int pageNumber,
        int pageSize,
        params Expression<Func<Domain.Entities.Order, object>>[] includes)
    {
        return await _unitOfWork.Repository<Domain.Entities.Order>().GetAllAsync(pageNumber, pageSize, includes);
    }
}
