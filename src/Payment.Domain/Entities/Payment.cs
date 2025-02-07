using Shared.Kernel.Core.Enuns;

namespace Billing.Domain.Entities;


public class Payment : Entity
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public int Status { get; set; }
    public CreditCard? CreditCard { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? TransactionId { get; set; }
}
