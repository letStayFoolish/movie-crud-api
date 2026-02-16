// This file is part of the project. Copyright (c) Company.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

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
