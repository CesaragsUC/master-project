

namespace Order.Domain.Entities;


public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    public OrderStatus Status { get; set; }
}
