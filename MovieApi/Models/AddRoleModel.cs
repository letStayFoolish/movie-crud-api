// This file is part of the project. Copyright (c) Company.

using Microsoft.Build.Framework;

namespace MovieApi.Models;

public class AddRoleModel
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Role { get; set; }
}
