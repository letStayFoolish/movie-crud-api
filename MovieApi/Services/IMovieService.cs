using MovieApi.DTOs.Movie;

namespace MovieApi.Services;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto movie, CancellationToken cancellationToken = default);
    Task<MovieDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync(CancellationToken cancellationToken = default);
    Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie, CancellationToken cancellationToken = default);
    Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default);
}