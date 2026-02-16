// One-To-One: Customer -> CustomerProfile

using MovieApi.Persistence;

namespace MovieApi.Models;

public class CustomerProfile
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    //The entity with FK is dependent. Here CustomerProfile depends on Customer!!! - A CustomerProfile couldn't exist without a Customer
    public Guid CustomerId { get; set; } // Foreign key to the Customer

    public Customer Customer { get; set; } = null!; // 1:1 Navigation property to the Customer entity
}
