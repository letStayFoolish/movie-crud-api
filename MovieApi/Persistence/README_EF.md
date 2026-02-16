# EF Core Relationships - One-to-One, One-to-Many, Many-to-Many
`HasOne`, `HasMany`, `WithOne`, `WithMany`

## What Are Relationships in EF Core?

**A relationship in Entity Framework Core defines how two entities are connected through a foreign key and navigation properties.** EF Core maps the references between C# objects to foreign key constraints in the database, keeping both sides in sync automatically.

### Terminology

**Principal Entity**: The entity that contains the primary key being referenced (e.g., `Customer`).

**Dependent Entity**: The entity that contains foreign key (e.g., `Order`).

**Navigation Property**: A C# property that references the related entity (e.g., `Order.Customer` or `Customer.Orders`).

**Foreign Key Property**: The property in the dependent that stores the principal's key value (e.g., `Order.CustomerId`).

### EF Core supports three types of relationships:
- One-to-One: Customer -> CustomerProfile: Foreign key location on **either entity (typically the dependent)**.
- One-to-Many: Customer -> Orders: Foreign key location **on the "many" side** (dependent).
- Many-to-Many: Product <-> Tag: Foreign key location **in a join table** (separate or implicit).

## Configuring One-to-Many Relationships

### Convention vs Fluent API

**The default delete behaviour is set to `Cascade`**. Deleting Customer all its Orders vanish silently.

**Fluent API allows you to override the default conventions and configure relationships explicitly**.

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
               .HasPrecision(18, 2);

        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.CustomerId);
    }
}
```

Here’s what the fluent chain does:
* `HasOne(o => o.Customer)` — This `Order` has one related `Customer`
* `WithMany(c => c.Orders)` — That `Customer` has many `Order` entities
* `HasForeignKey(o => o.CustomerId)` — The **`FK`** linking them
* `OnDelete(DeleteBehavior.Restrict)` — Prevent deleting a customer who has orders.

**PRO TIP: Always be explicit about delete behavior! The _default_ `Cascade` for required relationships can accidently wipe out important data. Use `Restrict` for business entities and handle delete logic in the application.**

## Querying One-to-Many

In a one-to-one relationship, each entity on both sides is associated with at most one entity on the other side. Think: a customer has exactly one profile, or an order has exactly one shipping address record.

```csharp
public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("customer_profiles");

        builder.HasKey(cp => cp.Id);

        builder.Property(cp => cp.PhoneNumber).HasMaxLength(20);

        builder.Property(cp => cp.ShippingAddress).HasMaxLength(500);

        builder.HasOne(cp => cp.Customer)
            .WithOne(c => c.CustomerProfile)
            .HasForeignKey<CustomerProfile>(cp => cp.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cp => cp.CustomerId)
            .IsUnique();
    }
}
```


- `WithOne` instead of `WithMany` — Both sides reference a single entity

- `HasForeignKey<CustomerProfile>` — You must specify the generic type parameter to tell EF Core which side is the **dependent**. This is required because in one-to-one relationships, EF Core can’t infer it automatically!

The unique index on `CustomerId` enforces the “one-to-one” constraint at the database level — each customer gets at most one profile.

_Cascade delete makes sense here because a profile has no meaning without its customer. But evaluate this case by case — not every one-to-one warrants cascading._

## Configuring Many-to-Many Relationships

### Simple Many-to-Many (No Payload)

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Price).HasPrecision(18, 2);

        builder.HasMany(p => p.Tags)
               .WithMany(t => t.Products)
               .UsingEntity(j => j.ToTable("ProductTags"));
    }
}
```

The `UsingEntity` call is optional but lets you control the join table name. Without it, EF Core auto-generates one.

Adding and querying tags is straightforward:
```csharp
var product = await dbContext.Products.FindAsync(productId, cancellationToken);
var tag = await dbContext.Tags.FindAsync(tagId, cancellationToken);

product!.Tags.Add(tag!);
await dbContext.SaveChangesAsync(cancellationToken);

// Query products with their tags
var productsWithTags = await dbContext.Products
    .Include(p => p.Tags)
    .ToListAsync(cancellationToken);
```
### Many-to-Many with Payload (Join Entity)

## Understanding Cascade Delete Behavior

| Behavior | What Happens | Use When |
| --- | --- | --- |
| Cascade | Delete parent → delete all children | Children meaningless without parent (OrderItems) |
| Restrict | Delete parent → throw exception if children exist | Children have independent business value (Products) |
| SetNull | Delete parent → set FK to null (FK must be nullable) | Optional relationships, children can exist alone |
| NoAction | Database decides (usually throws) | Rare — when you handle everything manually |

**Recommendation**: use **Fluent API** for all relationships. It’s explicit, self-documenting, and prevents surprises when conventions don’t match your intent. If you’re using a layered architecture with a repository pattern, keeping relationship configuration in dedicated `IEntityTypeConfiguration` classes keeps your persistence layer clean. And if you need raw SQL performance for read-heavy queries while keeping EF Core for writes, check out Using Entity Framework Core and Dapper — the hybrid approach works well alongside proper relationship configuration.

## Common Mistakes with EF Core Relationships

### Mistake 1: Configuring the Same Relationship from Both Sides

```csharp
// In OrderConfiguration
builder.HasOne(o => o.Customer).WithMany(c => c.Orders);

// In CustomerConfiguration — DON'T DO THIS
builder.HasMany(c => c.Orders).WithOne(o => o.Customer);
```

Both lines describe the same relationship. Configure it once, from one side — typically the dependent (the entity with the FK).

### Mistake 2: Missing Navigation Properties

```csharp
public class Order
{
    public Guid CustomerId { get; set; }
    // Missing: public Customer Customer { get; set; }
}
```
Without navigation properties, you can’t use `Include()` for eager loading or navigate relationships in LINQ queries. Always add them on at least one side — ideally both. This also impacts how you serialize data in your API responses — use DTOs to control what gets returned.

### Mistake 3: Forgetting Indexes on Foreign Keys

EF Core creates indexes on foreign keys automatically in most cases. But for composite foreign keys or custom scenarios, always verify by reviewing your migrations:
```bash
dotnet ef migrations add VerifyRelationships
dotnet ef migrations script
```
### Mistake 4: Cascade Delete on Self-Referencing Relationships

If you have a `Category` with a `ParentCategoryId` pointing to itself, never use cascade delete. It would recursively delete all children, their children, and so on — potentially wiping your entire category tree.

## Summary
**EF Core relationships map the connections between your C# entities to foreign key constraints in the database.** The three types — one-to-one, one-to-many, and many-to-many — cover every real-world data modeling scenario you’ll encounter.

Key takeaways:

- **One-to-many** is the most common — use `HasOne().WithMany().HasForeignKey()`,
- **One-to-one** requires specifying the dependent side with `HasForeignKey<T>()`,
- Simple **many-to-many** (EF Core 5+) needs no join entity — just collection navigation properties on both sides,
- **Many-to-many** with payload requires an explicit join entity with its own configuration
- Always set `OnDelete` **explicitly** — cascade defaults can destroy production data
Configure relationships from one side only — typically the dependent entity.

### Sources:
- [EF Core Relationships - One-to-One, One-to-Many, Many-to-Many](https://codewithmukesh.com/blog/ef-core-relationships-one-to-one-one-to-many-many-to-many/)
- [Introduction to relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships)
- [Cascade Delete](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete)
