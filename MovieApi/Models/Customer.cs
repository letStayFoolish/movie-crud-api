// One-To-Many: Customer -> Orders
// One-To-One: Customer -> CustomerProfile

using MovieApi.Models;

namespace MovieApi.Persistence;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    //Navigation Properties:
    public ICollection<Order> Orders { get; set; } = []; // 1:N new List<Order>(); == []
    public CustomerProfile? CustomerProfile { get; set; } // 1:1
}
