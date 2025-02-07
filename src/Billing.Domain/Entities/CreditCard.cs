namespace Billing.Domain.Entities;

public class CreditCard : Entity
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpirationDate { get; set; }
    public string? SecurityCode { get; set; }

}
