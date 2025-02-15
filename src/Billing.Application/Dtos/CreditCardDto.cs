using System.Diagnostics.CodeAnalysis;

namespace Billing.Application.Dtos;

[ExcludeFromCodeCoverage]
public class CreditCardDto
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpirationDate { get; set; }
    public string? SecurityCode { get; set; }
}
