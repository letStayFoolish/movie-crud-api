// This file is part of the project. Copyright (c) Company.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MovieApi.Constants;
using MovieApi.DTOs.Users;
using MovieApi.Enums;
using MovieApi.Models;
using MovieApi.Settings;

namespace MovieApi.Services.Users;

public class UsersService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JWT _jwt;
    private readonly ILogger<UsersService> _logger;

    public UsersService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,
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
            LastName = model.LastName
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
}
