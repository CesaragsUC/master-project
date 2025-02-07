using System.Diagnostics.CodeAnalysis;

namespace Application.Dtos.Dtos.Payments;

[ExcludeFromCodeCoverage]
public class CreditCardDto
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpirationDate { get; set; }
    public string? SecurityCode { get; set; }
}
