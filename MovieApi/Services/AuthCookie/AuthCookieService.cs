namespace MovieApi.Services.AuthCookie;

public class AuthCookieService : IAuthCookieService
{
    private const string CookieName = "refreshToken";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public void SetRefreshTokenInCookie(string refreshToken, int expiresUtc)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            // Secure = true, // set true in production (HTTPS)
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(expiresUtc)
        };

        httpContext.Response.Cookies.Append(CookieName, refreshToken, cookieOptions);
    }

    public void ClearRefreshTokenCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");

        httpContext.Response.Cookies.Delete(CookieName);
    }
}
