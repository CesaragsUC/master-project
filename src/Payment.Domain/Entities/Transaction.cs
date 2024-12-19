namespace Payment.Domain.Entities;

public class Transaction
{
    public string? AuthorizationCode { get; set; }
    public string? CreditCardCompany { get; set; }
    public DateTime? TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public decimal TransactionCost { get; set; }
    public PaymentStatus TransactionStatus { get; set; }
    public string? TID { get; set; } // Id
    public string? NSU { get; set; } // Meio (paypal)

    public Guid PaymentId { get; set; }

    // EF Relation
    public Payment? Payment { get; set; }
}
