using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Enums;
using MovieApi.Filters;
using MovieApi.Middlewares;
using MovieApi.Models;
using MovieApi.Persistence;
using MovieApi.Services.AuthCookie;
using MovieApi.Services.Movies;
using MovieApi.Services.Notifications;
using MovieApi.Services.RefreshToken;
using MovieApi.Services.Users;
using MovieApi.Settings;
using MovieApi.Utilities;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace MovieApi.Extensions;

public static class DIExtensions
{
    public static IServiceCollection AddMovieApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<LoggingFilter>(); // register the filter itself

        // Authentication/Authorization
        // Configuration from AppSettings
        // services.Configure<JWT>(Configuration.GetSection("JWT"));
        services.AddOptions<JWT>()
            .BindConfiguration("JWT")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        // User Manager Service
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // consider false only for local dev without HTTPS
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
                };
            });
        services.AddAuthorization(); // add policies here if needed

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddResponseCompression();
        services.AddSession();

        services.AddOptions<PaginationOptions>().BindConfiguration(nameof(PaginationOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart(); // Startup Validation - you don’t want your application to even boot up if there are validation issues with the configurations.

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        //Scoped lifetime for DbContext (default when using AddDbContext)
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });
        services.AddHttpContextAccessor();
        services.AddTransient<IMovieService, MovieService>();
        services.AddKeyedScoped<INotificationService, EmailNotificationService>(NotificationChannel.Email);
        services.AddKeyedScoped<INotificationService, SmsNotificationService>(NotificationChannel.Sms);
        services.AddScoped<NotificationHandler>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthCookieService, AuthCookieService>();

        services.Configure<ApiBehaviorOptions>(o =>
                o.SuppressModelStateInvalidFilter = true // let ValidateModelFilter handle invalid ModelState
        );
        services.Configure<RouteOptions>(o =>
            o.LowercaseUrls = true
        );
        services.Configure<JsonOptions>(o =>
        {
            o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            o.SerializerOptions.WriteIndented = false;
        });

        return services;
    }
}
