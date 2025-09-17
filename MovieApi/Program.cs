using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.Enums;
using MovieApi.Filters;
using MovieApi.Middlewares;
using MovieApi.Models;
using MovieApi.Persistence;
using MovieApi.Services.Movies;
using MovieApi.Services.Notifications;
using MovieApi.Utilities;
using Scalar.AspNetCore;
using Serilog;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

//Source: https://codewithmukesh.com/blog/aspnet-core-webapi-crud-with-entity-framework-core-full-course/#crud-operations---movieservice-implementation
try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    Log.Information("Starting up the server... 💡");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, config) =>
    {
        config.WriteTo.Console();
        config.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddScoped<LoggingFilter>(); // register the filter itself

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<LoggingFilter>(); // resolve from DI per request (scoped)
    });
    builder.Services.AddOptions<PaginationOptions>().BindConfiguration(nameof(PaginationOptions));
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddDbContext<MovieDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString);
    });
    builder.Services.AddTransient<IMovieService, MovieService>();
    builder.Services.AddKeyedScoped<INotificationService, EmailNotificationService>(NotificationChannel.Email);
    builder.Services.AddKeyedScoped<INotificationService, SmsNotificationService>(NotificationChannel.Sms);
    builder.Services.AddScoped<NotificationHandler>();

    builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);
    builder.Services.Configure<JsonOptions>(o =>
    {
        o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.SerializerOptions.WriteIndented = false;
    });
    var app = builder.Build();

    await using (var serviceScope = app.Services.CreateAsyncScope())
    await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<MovieDbContext>())
    {
        // This ensures that the database is in place before any data operations are performed, preventing errors that might occur if the database is missing
        // This pattern is typically used in applications where database creation is required at startup but where migrations are not being used
        await dbContext.Database.EnsureCreatedAsync();

        // Recommended Approach for Production
        // Use EF Core Migrations instead of EnsureCreatedAsync. Migrations provide a systematic way to evolve your database schema while preserving existing data.
        // await dbContext.Database.MigrateAsync(cancellationToken: default);
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseExceptionHandler();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    // Endpoints
    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
