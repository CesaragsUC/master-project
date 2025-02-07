

namespace Order.Domain.Entities;

public enum OrderStatus
{
    Created = 1,
    Pending = 2,
    Confirmed = 3,
    Shipped = 4,
    Delivered = 5,
    Canceled = 6
}
