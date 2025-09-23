using MovieApi.DTOs.Users;

namespace MovieApi.Services.Users;

// ...a Services class that contains the Core User Functions like Register, Generate JWTs etc.
public interface IUsersService
{
    public Task<string> RegisterAsync(RegisterModel model);
}
