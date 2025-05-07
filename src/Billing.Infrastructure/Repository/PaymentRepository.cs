using Billing.Domain.Abstractions;
using Billing.Domain.Entities;
using HybridRepoNet.Abstractions;
using System.Linq.Expressions;

namespace Billing.Infrastructure.Repository;

public class PaymentRepository : IPaymentRepository
{
    public readonly IUnitOfWork<BillingContext> _unitOfWork;

    public PaymentRepository(IUnitOfWork<BillingContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task AddAsync(Payment payment)
    {
        return _unitOfWork.Repository<Payment>().AddAsync(payment);
    }

    public async Task<bool> Commit()
    {
        return await _unitOfWork.Commit();
    }

    public void Delete(Payment payment)
    {
        _unitOfWork.Repository<Payment>().Delete(payment);
    }

    public async Task<Payment?> FindAsync(Expression<Func<Payment, bool>> wherePredicate)
    {
        var payment = await _unitOfWork.Repository<Payment>()
            .FindAsync(wherePredicate);
        return payment;
    }

    public Payment? FindOne(Guid id)
    {
        var payment = _unitOfWork.Repository<Payment>()
            .FindOne(x => x.Id == id);
        return payment;
    }

    public Task<IEnumerable<Payment>> GetAllAsync()
    {
        var payments = _unitOfWork.Repository<Payment>()
            .GetAllAsync();
        return payments;
    }
    public void SoftDelete(Payment payment)
    {
        _unitOfWork.Repository<Payment>().Delete(payment);
    }

    public async Task SoftDelete(Expression<Func<Payment, bool>> expression)
    {
        await _unitOfWork.Repository<Payment>().SoftDeleteAsync(expression);
    }

    public void Update(Payment payment)
    {
        _unitOfWork.Repository<Payment>().Update(payment);
    }
}
