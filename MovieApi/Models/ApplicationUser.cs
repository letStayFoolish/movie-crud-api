// This file is part of the project. Copyright (c) Company.

using Microsoft.AspNetCore.Identity;

namespace MovieApi.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
