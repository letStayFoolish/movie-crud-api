// This file is part of the project. Copyright (c) Company.

using MovieApi.Persistence;

namespace MovieApi.Models;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    public Guid CustomerId { get; set; } // Foreign key
    public Customer Customer { get; set; } = null!; // N:1 | one to many (Navigation Property)

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // 1:N (join)
}
