using MovieApi.DTOs.Auth;
using MovieApi.DTOs.Users;
using MovieApi.Models;

namespace MovieApi.Services.Users;

// ...a Services class that contains the Core User Functions like Register, Generate JWTs etc.
public interface IUsersService
{
    public Task<string> RegisterAsync(RegisterModel model);
    public Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model);
    public Task<string> AddRoleAsync(AddRoleModel model);
    public Task<AuthenticationModel> RefreshTokenAsync(string token);
    public Task<bool> RevokeTokenAsync(string token);
    public ApplicationUser? GetById(string id);
}
