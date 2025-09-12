using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Persistence;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var sampleMovie = await context.Set<Movie>()
                    .FirstOrDefaultAsync(b => b.Title == "Sonic the Hedgehog 3", cancellationToken);
                if (sampleMovie is null)
                {
                    sampleMovie = Movie.Create("Sonic the Hedgehog 3", "Fantasy",
                        new DateTimeOffset(new DateTime(2025, 1, 3), TimeSpan.Zero), 9.5);
                    await context.Set<Movie>().AddAsync(sampleMovie, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
            })
            .UseSeeding((context, _) =>
            {
                var sampleMovie = context.Set<Movie>()
                    .FirstOrDefault(b => b.Title == "Sonic the Hedgehog 3");
                if (sampleMovie is null)
                {
                    sampleMovie = Movie.Create("Sonic the Hedgehog 3", "Fantasy",
                        new DateTimeOffset(new DateTime(2025, 1, 3), TimeSpan.Zero), 9.5);
                    context.Set<Movie>().Add(sampleMovie);
                    context.SaveChanges();
                }
            });
    }
}
