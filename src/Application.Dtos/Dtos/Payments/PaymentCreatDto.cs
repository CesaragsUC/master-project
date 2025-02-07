using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Payments;

[ExcludeFromCodeCoverage]
public class PaymentCreatDto
{

    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public CreditCardDto? CreditCard { get; set; }
}
