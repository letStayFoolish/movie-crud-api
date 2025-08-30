namespace MovieApi.DTOs.Movie;

public record MovieDto(Guid Id, string Title, string Genre, DateTimeOffset ReleaseDate, double Rating);