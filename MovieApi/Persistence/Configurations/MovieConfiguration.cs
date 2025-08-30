using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

// The MovieConfiguration class implements IEntityTypeConfiguration<Movie>, allowing us to define how the Movie entity should be mapped to the database.
// This configuration is cleanly encapsulated in the Configure method.
public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        // Define table name
        builder.ToTable("movies");
        
        // Set primary key
        builder.HasKey(m => m.Id);
        
        // Configure properties
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(m => m.Genre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.ReleaseDate)
            .IsRequired();

        builder.Property(m => m.Rating)
            .IsRequired();

        // Configure Created and LastModified properties to be handled as immutable and modifiable timestamps
        builder.Property(m => m.Created)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(m => m.LastModified)
            .IsRequired()
            .ValueGeneratedOnUpdate();
        
        // Optional: add index for better query performance
        // creates a database index on the Title column. This can significantly improve query performance when searching or filtering by Title
        builder.HasIndex(m => m.Title);
    }
}