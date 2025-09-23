using Microsoft.AspNetCore.Identity;
using MovieApi.Constants;
using MovieApi.Enums;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

public static class IdentitySeeder
{
    private static readonly string[] Roles =
    [
        "Administrator",
        "Moderator",
        "User"
    ];

    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string username = Authorization.default_username;
        const string email = Authorization.default_email;
        const string password = Authorization.default_password;
        const Roles defaultRole = Authorization.default_role;
        var existing = await userManager.FindByEmailAsync(email);

        if (existing is null)
        {
            var user = new ApplicationUser
            {
                FirstName = "UserFirstName",
                LastName = "UserLastName",
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var create = await userManager.CreateAsync(user, password);
            if (create.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                // log
            }

            // log errors
        }
    }
}
