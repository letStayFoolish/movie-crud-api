namespace MovieApi.Models;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = []; // new List<Product>() == [];
}
//Both sides have a collection navigation property. EF Core detects this and creates a ProductTag join table behind the scenes.
