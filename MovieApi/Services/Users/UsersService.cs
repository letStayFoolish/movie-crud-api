using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieApi.Constants;
using MovieApi.DTOs.Auth;
using MovieApi.DTOs.Users;
using MovieApi.Enums;
using MovieApi.Exceptions;
using MovieApi.Models;
using MovieApi.Services.RefreshToken;
using MovieApi.Settings;

namespace MovieApi.Services.Users;

public class UsersService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWT _jwt;
    private readonly IRefreshTokenService _refreshTokenService;

    public UsersService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
        if (userWithSameEmail == null)
        {
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Authorization.default_role.ToString());
            }

            return $"User Registered with username {user.UserName}";
        }
        else
        {
            return $"Email {user.Email} already exists";
        }
    }

    public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
    {
        var authenticationModel = new AuthenticationModel();
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"No Account Registered with {model.Email}.";
            return authenticationModel;
        }

        if (await _userManager.CheckPasswordAsync(user, model.Password))
        {
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtTokenAsync(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // if (user.Email is null || user.UserName is null)
            // {
            //     authenticationModel.IsAuthenticated = false;
            //     authenticationModel.Message = $"No Email or Username for user {user.Email}.";
            //     return authenticationModel;
            // }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                authenticationModel.IsAuthenticated = false;
                throw new UserDataException(user.Email);
            }


            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                authenticationModel.IsAuthenticated = false;
                throw new UserDataException(user.UserName);
            }

            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            var roleList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = roleList.ToList();

            if (user.RefreshTokens.Any(a => a.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authenticationModel.RefreshToken = activeRefreshToken.Token;
                authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
            }
            else
            {
                var refreshToken = _refreshTokenService.CreateRefreshToken();

                authenticationModel.RefreshToken = refreshToken.Token;
                authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authenticationModel;
        }

        authenticationModel.IsAuthenticated = false;
        authenticationModel.Message = $"Incorrect Credentials for user {user.Email}.";
        return authenticationModel;
    }

    public async Task<string> AddRoleAsync(AddRoleModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            return $"No Accounts Registered with {model.Email}.";
        }

        if (await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roleExists = Enum.GetNames(typeof(Roles)).Any(x => x.ToLower() == model.Role.ToLower());
            if (roleExists)
            {
                var validRole = Enum.GetValues(typeof(Roles)).Cast<Roles>().FirstOrDefault(x =>
                    string.Equals(x.ToString(), model.Role, StringComparison.CurrentCultureIgnoreCase));
                await _userManager.AddToRoleAsync(user, validRole.ToString());
                return $"Added {model.Role} to user {model.Email}.";
            }

            return $"Role {model.Role} not found.";
        }

        return $"Incorrect Credentials for user {user.Email}.";
    }

    public async Task<AuthenticationModel> RefreshTokenAsync(string token)
    {
        var authenticationModel = new AuthenticationModel();
        var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user == null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Token did not match any users.";
            return authenticationModel;
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Token Not Active.";
            return authenticationModel;
        }

        // Revoke Current Refresh Token
        refreshToken.Revoked = DateTime.UtcNow; // This makes the refresh token inactive.

        // Generate new Refresh Token and save to Database
        var newRefreshToken = _refreshTokenService.CreateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);

        //Generates new JWT
        authenticationModel.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = await CreateJwtTokenAsync(user);
        authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authenticationModel.Email = user.Email;
        authenticationModel.UserName = user.UserName;
        var roleList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        authenticationModel.Roles = roleList.ToList();
        authenticationModel.RefreshToken = newRefreshToken.Token;
        authenticationModel.RefreshTokenExpiration = newRefreshToken.Expires;
        return authenticationModel;
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user is null) return false;

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if(!refreshToken.IsActive) return false;

        // revoke token and save
        refreshToken.Revoked = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public ApplicationUser? GetById(string id)
    {
        return _userManager.Users.SingleOrDefault(u => u.Id == id);
    }

    private async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();
        // or:
        /*
         *   var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }2
         */
        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }
}
