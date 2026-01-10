using MovieApi.Middlewares;

namespace MovieApi.Extensions;

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpRequestLogger>();
    }
}
