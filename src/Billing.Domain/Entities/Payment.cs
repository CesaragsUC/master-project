using HybridRepoNet;
using System.ComponentModel.DataAnnotations.Schema;

namespace Billing.Domain.Entities;


public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public decimal Amount { get; set; }

    public int Method { get; set; }

    public int Status { get; set; }
    
    [NotMapped]
    public CreditCard? CreditCard { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? TransactionId { get; set; }
}
