// This file is part of the project. Copyright (c) Company.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict); // we prevent deleting orders to also delete the customer. If Order has customer delete would not be possible!

        builder.HasIndex(o => o.CustomerId);
    }
}

/*
 * Here’s what the fluent chain does:
 * HasOne(o => o.Customer) — This Order has one related Customer
 * WithMany(c => c.Orders) — That Customer has many Order entities
 * HasForeignKey(o => o.CustomerId) — The FK linking them
 * OnDelete(DeleteBehavior.Restrict) — Prevent deleting a customer who has orders
*/
