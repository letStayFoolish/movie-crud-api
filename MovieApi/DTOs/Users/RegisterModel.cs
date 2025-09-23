// This file is part of the project. Copyright (c) Company.

using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Users;

public class RegisterModel
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
