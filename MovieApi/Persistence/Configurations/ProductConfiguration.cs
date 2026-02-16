// This file is part of the project. Copyright (c) Company.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Price).HasPrecision(18, 2);

        builder.HasMany(p => p.Tags)
            .WithMany(t => t.Products)
            .UsingEntity(j => j.ToTable("product_tags"));//The UsingEntity call is optional but lets you control the join table name. Without it, EF Core auto-generates one.
    }
}
