namespace MovieApi.Services.AuthCookie;

public interface IAuthCookieService
{
    public void SetRefreshTokenInCookie(string token, int expiresUtc);
    public void ClearRefreshTokenCookie();
}
