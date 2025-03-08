namespace Shared.Kernel.Core.Enuns;

public enum OrderStatus
{
    Created = 1,
    Cancelled = 2,
    Completed = 3,
    Shipped = 4,
    Delivered = 5,
    Returned = 6,
    Refunded = 7,
    PartiallyRefunded = 8,
    PaymentFailed = 9,
    Pending = 10,
}