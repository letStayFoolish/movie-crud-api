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
using MovieApi.Settings;

namespace MovieApi.Services.Users;

public class UsersService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;
    private readonly ILogger<UsersService> _logger;

    public UsersService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IOptions<JWT> jwt,
        ILogger<UsersService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
        _logger = logger;
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

            if (String.IsNullOrWhiteSpace(user.Email))
            {
                authenticationModel.IsAuthenticated = false;
                throw new UserDataException(user.Email);
            }


            if (String.IsNullOrWhiteSpace(user.UserName))
            {
                authenticationModel.IsAuthenticated = false;
                throw new UserDataException(user.UserName);
            }

            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            var roleList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = roleList.ToList();
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
                var validRole = Enum.GetValues(typeof(Roles)).Cast<Roles>().FirstOrDefault(x => string.Equals(x.ToString(), model.Role, StringComparison.CurrentCultureIgnoreCase));
                await _userManager.AddToRoleAsync(user, validRole.ToString());
                return $"Added {model.Role} to user {model.Email}.";
            }
            return $"Role {model.Role} not found.";
        }
        return $"Incorrect Credentials for user {user.Email}.";
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
            }
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
            expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }
}
