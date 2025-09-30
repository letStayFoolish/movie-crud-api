using System.Security.Cryptography;

namespace MovieApi.Services.RefreshToken;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly int ExpiresInDays = 10;

    public Models.RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = new RNGCryptoServiceProvider())
        {
            generator.GetBytes(randomNumber);
            return new Models.RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(ExpiresInDays),
                Created = DateTime.UtcNow,
            };
        }
    }
}
