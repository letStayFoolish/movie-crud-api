using MovieApi.DTOs.Movie;

namespace MovieApi.Services;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto movie);
    Task<MovieDto?> GetMovieByIdAsync(Guid movieId);
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie);
    Task DeleteMovieAsync(Guid movieId);
}