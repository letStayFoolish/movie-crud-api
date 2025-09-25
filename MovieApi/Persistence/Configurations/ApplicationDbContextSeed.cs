using Microsoft.AspNetCore.Identity;
using MovieApi.Constants;
using MovieApi.Enums;
using MovieApi.Models;

namespace MovieApi.Persistence.Configurations;

public class ApplicationDbContextSeed
{
    public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed roles
        await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

        // Seed Default user
        const string firstName = "Default";
        const string lastName = "User";
        const string email = Authorization.default_email;
        const string username = Authorization.default_username;
        const string password = Authorization.default_password;
        string defaultRole = Authorization.default_role.ToString();

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is null)
        {
            var defaultUser = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var create = await userManager.CreateAsync(defaultUser, password);
            if(create.Succeeded)
            {
                // Ensure the role exists (safety) then add
                if (!await roleManager.RoleExistsAsync(defaultRole))
                {
                    await roleManager.CreateAsync(new IdentityRole(defaultRole));
                }

                await userManager.AddToRoleAsync(defaultUser, defaultRole);
            }
        }
    }
}
