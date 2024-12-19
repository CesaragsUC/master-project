

namespace Order.Domain.Entities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Canceled
}
