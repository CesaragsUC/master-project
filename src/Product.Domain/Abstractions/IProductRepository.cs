namespace Product.Domain.Abstractions;

public interface IProductRepository
{
    Task AddAsync(Models.Product produto);
    Models.Product? FindOne(Guid id);
    Task<IEnumerable<Models.Product>> GetAllAsync();
    void Delete(Models.Product produto);
    void Update(Models.Product produto);
    public Task<bool> Commit();
}
