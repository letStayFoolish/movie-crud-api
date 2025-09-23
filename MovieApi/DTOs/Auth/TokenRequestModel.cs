using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Auth;

public record TokenRequestModel(
    [Required] string Email,
    [Required] string Password
);
