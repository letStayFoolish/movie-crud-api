using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Movie;

public record UpdateMovieDto(
    [MaxLength(200)]
    string? Title,
    [MaxLength(100)]
    string? Genre,
    DateTimeOffset? ReleaseDate,
    [param: Range(0, 10)]
    double? Rating
);

