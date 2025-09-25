using System.Net;
using Microsoft.AspNetCore.Identity;
using MovieApi.Enums;

namespace MovieApi.Models;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
