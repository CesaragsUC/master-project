using Billing.Application.Dtos;
using ResultNet;

namespace Billing.Application.Service;

public interface IPaymentService
{
    Task<Result<bool>> CreatePaymentAsync(PaymentCreatDto paymentDto);
    Task<Result<PaymentDto>> GetPaymentAsync(string transactionId);
    Task<Result<bool>> DeletePaymentAsync(string transactionId);
    Task<Result<IEnumerable<PaymentDto>>> GetAllPaymentAsync();
}
