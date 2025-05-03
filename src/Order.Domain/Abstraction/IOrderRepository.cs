using System.Linq.Expressions;

namespace Order.Domain.Abstraction;

public interface IOrderRepository
{
    Task AddAsync(Entities.Order order);

    Task AddAsync(IEnumerable<Entities.OrderItem> orderItem);

    Entities.Order? FindOne(Guid id);

    Task<Entities.Order?> FindAsync(Expression<Func<Entities.Order, bool>> wherePredicate,
         params Expression<Func<Entities.Order, object>>[] includes);

    Task<IEnumerable<Entities.Order?>> GetAllAsync(Expression<Func<Entities.Order, bool>> wherePredicate,
         params Expression<Func<Entities.Order, object>>[] includes);

    Task<IEnumerable<Entities.Order?>> GetAllAsync(
        int pageNumber,
        int pageSize,
        params Expression<Func<Entities.Order, object>>[] includes);

    Task<Domain.Entities.Order?> FindAsync(Expression<Func<Entities.Order, bool>> wherePredicate);

    Task<IEnumerable<Entities.Order>> GetAllAsync();

    void Delete(Entities.Order order);

    public void SoftDelete(Entities.Order order);

    void Update(Entities.Order order);

    public Task<bool> Commit();
}
