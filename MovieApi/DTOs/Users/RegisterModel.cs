// This file is part of the project. Copyright (c) Company.

using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Users;

public record RegisterModel(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Username,
    [Required] string Email,
    [Required] string Password
);
