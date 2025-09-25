using System.ComponentModel.DataAnnotations;
using MovieApi.Enums;

namespace MovieApi.DTOs.Users;

public record RegisterModel(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Username,
    [Required] string Email,
    [Required] string Password
);
