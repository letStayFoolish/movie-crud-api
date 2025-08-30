namespace MovieApi.DTOs.Movie;

public record CreateMovieDto(string Title, string Genre, DateTimeOffset ReleaseDate, double Rating);