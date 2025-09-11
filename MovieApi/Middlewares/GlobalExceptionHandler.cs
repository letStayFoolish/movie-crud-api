using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Exceptions;

namespace MovieApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Log with correlation
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }

        // 2) Map extension -> HTTP status + title/type
        var (status, title) = MapExtensions(exception);
        
        // 3) Build ProblemDetails (do not leak internals in prod)
        var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
        var problemDetail = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = exception.GetType().FullName,
            Detail = env.IsDevelopment() ? exception.ToString() : null,
            Instance = httpContext.Request.Path
        };
            
        // 4) Enrich universally useful metadata
        problemDetail.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetail.Extensions["timestamp"] = DateTimeOffset.UtcNow.ToString("o");
        
        httpContext.Response.StatusCode = status;
        
        // 5. Write response
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetail,
        });

        return true;
    }

    private static (int Status, string Title) MapExtensions(Exception ex)
    {
        // Map known exception types from your domain/infrastructure
        // Add your custom exceptions below to control status/title/type
        return ex switch
        {
            // Bad input
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument"),
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "Argument out of range"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),

            // Validation (System.ComponentModel.DataAnnotations)
            System.ComponentModel.DataAnnotations.ValidationException =>
                (StatusCodes.Status400BadRequest, "Validation failed"),

            // Validation (FluentValidation)
            // FluentValidation.ValidationException =>
            //     (StatusCodes.Status400BadRequest, "Validation failed"),

            // AuthZ/AuthN
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            // Not found
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),

            // Concurrency / conflicts (EF Core)
            Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException =>
                (StatusCodes.Status409Conflict, "Concurrency conflict"),

            // Upstream HTTP failures
            HttpRequestException httpEx when httpEx.StatusCode is not null =>
                ((int)httpEx.StatusCode.Value, "Upstream HTTP error"),

            // Timeouts / cancellations
            TimeoutException => (StatusCodes.Status504GatewayTimeout, "Operation timed out"),
            OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Operation canceled"),

            // Not supported/implemented
            NotSupportedException => (StatusCodes.Status405MethodNotAllowed, "Operation not supported"),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Not implemented"),
            
            // Custom exceptions
            MovieNotFoundException => (StatusCodes.Status404NotFound, "Movie not found"),

            // Catch-all
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
}