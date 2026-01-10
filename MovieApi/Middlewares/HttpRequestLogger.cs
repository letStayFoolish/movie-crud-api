namespace MovieApi.Middlewares;

/// <summary>
/// Convention-Based Middleware
/// Represents a middleware component responsible for logging details about incoming HTTP requests,
/// including query strings, HTTP methods, request paths, and user agent information.
/// </summary>
public class HttpRequestLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpRequestLogger> _logger;

    public HttpRequestLogger(RequestDelegate next,
        ILogger<HttpRequestLogger> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string query = context.Request.QueryString.ToString();
        string method = context.Request.Method;
        string path = context.Request.Path.ToString();
        string userAgent = context.Request.Headers["User-Agent"].ToString();

        if (!String.IsNullOrEmpty(query))
        {
            _logger.LogInformation($"Request query string: {query}");
        }

        if (!String.IsNullOrEmpty(method))
        {
            _logger.LogInformation($"Request method: {method}");
        }

        if (!String.IsNullOrEmpty(path))
        {
            _logger.LogInformation($"Request path: {path}");
        }

        if (!String.IsNullOrEmpty(userAgent))
        {
            _logger.LogInformation($"Request user agent: {userAgent}");
        }

        await _next(context);
    }
}
