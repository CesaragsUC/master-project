using RepoPgNet;
using ResultNet;

namespace Order.Service;

public class OrderService(IPgRepository<Domain.Entities.Order> repository) : IOrderService
{
    public async Task<Result<bool>> Add(Domain.Entities.Order order)
    {
        await repository.AddAsync(order);

        return await Result<bool>.SuccessAsync(true);
    }

    public async Task<Result<bool>> Delete(Guid id)
    {
        var entity = await repository.FindAsync(x => x.Id == id);
        if (entity is null)
        {
            return await Result<bool>.FailureAsync("Order not found");
        }
        await repository.DeleteAsync(entity);

        return await Result<bool>.SuccessAsync(true);
    }

    public async Task<Result<Domain.Entities.Order>> Get(Guid id)
    {
        var entity = await repository.FindAsync(x => x.Id == id);
        if (entity is null)
        {
            return await Result<Domain.Entities.Order>.FailureAsync("Order not found");
        }

        return await Result<Domain.Entities.Order>.SuccessAsync(entity);
    }

    public async Task<Result<IEnumerable<Domain.Entities.Order>>> List()
    {
        var list = await repository.GetAllAsync(1,2,x=> x.Items!);
        if (list is null)
        {
            return await Result<IEnumerable<Domain.Entities.Order>>.FailureAsync("Orders not found");
        }

        return await Result<IEnumerable<Domain.Entities.Order>>.SuccessAsync(list);
    }

    public async Task<Result<bool>> Update(Domain.Entities.Order order)
    {
        var entity = await repository.FindAsync(x => x.Id == order.Id);
        if (entity is null)
        {
            return await Result<bool>.FailureAsync("Order not found");
        }
        entity.CustomerId = order.CustomerId;
        entity.Items = order.Items;
        entity.Status = order.Status;

        await repository.UpdateAsync(entity);

        return await Result<bool>.SuccessAsync(true);
    }
}
