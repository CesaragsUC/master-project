

namespace Order.Domain.Entities;

public enum OrderStatus
{
    Created = 1,
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Canceled
}
