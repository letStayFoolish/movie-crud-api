namespace MovieApi.DTOs.Movie;

public record UpdateMovieDto(string Title, string Genre, DateTimeOffset ReleaseDate, double Rating);