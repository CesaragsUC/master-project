using System.ComponentModel.DataAnnotations.Schema;

namespace Billing.Domain.Entities;

[Table("Payments")]
public class Payment : Entity
{
    public Payment()
    {
        Transactions = new List<Transaction>();
    }

    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public CreditCard? CreditCard { get; set; }
    public DateTime PaymentDate { get; set; }

    // EF Relation
    public ICollection<Transaction> Transactions { get; set; }

    public void AdicionarTransacao(Transaction transaction)
    {
        Transactions.Add(transaction);
    }
}
