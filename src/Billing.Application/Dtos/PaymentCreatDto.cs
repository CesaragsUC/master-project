using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Dtos;

[ExcludeFromCodeCoverage]
public class PaymentCreatDto
{

    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public CreditCardDto? CreditCard { get; set; }
}
