using Application.Dtos.Dtos.Payments;
using ResultNet;

namespace Billing.Application.Service;

public interface IPaymentService
{
    Task<Result<bool>> CreatePaymentAsync(PaymentCreatDto payment);
    Task<Result<PaymentDto>> GetPaymentAsync(string transactionId);
    Task<Result<bool>> DeletePaymentAsync(string transactionId);
    Task<Result<IEnumerable<PaymentDto>>> GetAllPaymentAsync();
}
