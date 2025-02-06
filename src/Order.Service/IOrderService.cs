using ResultNet;

namespace Order.Service;

public interface IOrderService
{
    Task<Result<bool>> Add(Domain.Entities.Order order);
    Task<Result<bool>> Update(Domain.Entities.Order order);
    Task<Result<bool>> Delete(Guid id);
    Task<Result<Domain.Entities.Order>> Get(Guid id);
    Task<Result<IEnumerable<Domain.Entities.Order>>> List();
}