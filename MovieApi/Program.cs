using Microsoft.EntityFrameworkCore;
using MovieApi.Persistence;
using MovieApi.Services;
using Scalar.AspNetCore;
//Source: https://codewithmukesh.com/blog/aspnet-core-webapi-crud-with-entity-framework-core-full-course/#crud-operations---movieservice-implementation
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MovieDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});
builder.Services.AddTransient<IMovieService, MovieService>();

var app = builder.Build();

await using (var serviceScope = app.Services.CreateAsyncScope())
await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<MovieDbContext>())
{
    // This ensures that the database is in place before any data operations are performed, preventing errors that might occur if the database is missing
    // This pattern is typically used in applications where database creation is required at startup but where migrations are not being used
    await dbContext.Database.EnsureCreatedAsync();
    
    // Recommended Approach for Production
    // Use EF Core Migrations instead of EnsureCreatedAsync. Migrations provide a systematic way to evolve your database schema while preserving existing data.
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapMovieEndpoints();

app.MapControllers();

app.Run();