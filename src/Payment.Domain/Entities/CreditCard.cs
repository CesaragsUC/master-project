namespace Payment.Domain.Entities;

public class CreditCard
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpirationDate { get; set; }
    public string? SecurityCode { get; set; }

    protected CreditCard() { }

    public CreditCard(string holder, string cardNumber, string expirationDate, string securityCode)
    {
        Holder = holder;
        CardNumber = cardNumber;
        ExpirationDate = expirationDate;
        SecurityCode = securityCode;
    }
}
