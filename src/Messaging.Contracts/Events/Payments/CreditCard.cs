using System.Diagnostics.CodeAnalysis;

namespace Messaging.Contracts.Events.Payments;

[ExcludeFromCodeCoverage]
public class CreditCard 
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpirationDate { get; set; }
    public string? SecurityCode { get; set; }

}