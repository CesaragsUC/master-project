namespace Payment.Domain.Entities;

public enum PaymentStatus
{
    Authorized = 1,
    Pending,
    Completed,
    Failed,
    Refunded,
    Cancelled
}
