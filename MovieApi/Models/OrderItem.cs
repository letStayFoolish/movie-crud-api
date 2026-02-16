// Many-to-Many with Payload (Join Entity)

namespace MovieApi.Models;

public class OrderItem
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!; //forgiving-null: This operator tells the compiler: "I know this looks like it could be null, but trust me—treat it as non-null for static analysis purposes." It suppresses warnings about potential null values.

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Payload - Extra data on the relationship
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
//This is technically two one-to-many relationships joined through OrderItem.
