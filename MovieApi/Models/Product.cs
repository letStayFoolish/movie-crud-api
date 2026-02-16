// Many-to-Many (no payload): Product -> Tags

namespace MovieApi.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
//Both sides have a collection navigation property. EF Core detects this and creates a ProductTag join table behind the scenes.
