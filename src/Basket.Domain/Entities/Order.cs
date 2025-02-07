//namespace Basket.Domain.Entities;

//public class Order
//{
//    public Guid Id { get; set; } = Guid.NewGuid();

//    public Guid CustomerId { get; set; }

//    public int? Status { get; set; }

//    public List<OrderItem> Items { get; set; } = new();

//    public string? Name { get; set; }

//    public decimal SubTotal => Items.Sum(item => item.TotalPrice);

//    public decimal TotalAmount { get; set; }

//    public string? PaymentToken { get; set; }

//    public DateTime CreatedAt { get; set; } = DateTime.Now;

//}
