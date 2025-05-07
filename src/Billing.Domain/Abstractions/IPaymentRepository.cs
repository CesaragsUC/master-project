using Billing.Domain.Entities;
using System.Linq.Expressions;

namespace Billing.Domain.Abstractions;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Payment? FindOne(Guid id);
    Task<Payment?> FindAsync(Expression<Func<Payment, bool>> wherePredicate);
    Task<IEnumerable<Payment >> GetAllAsync();
    void Delete(Payment payment);
    void Update(Payment payment );
    void SoftDelete(Payment payment);
    Task SoftDelete(Expression<Func<Payment, bool>> expression);
    public Task<bool> Commit();
}
