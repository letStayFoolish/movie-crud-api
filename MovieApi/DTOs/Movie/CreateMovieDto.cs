using System.ComponentModel.DataAnnotations;

namespace MovieApi.DTOs.Movie;

public record CreateMovieDto(
    [param: Required(AllowEmptyStrings = false), MinLength(1), MaxLength(200)]
    string Title,

    [param: Required(AllowEmptyStrings = false), MinLength(1), MaxLength(100)]
    string Genre,

    [param: Required]
    DateTimeOffset ReleaseDate,

    [param: Range(0, 10)]
    double Rating
);

