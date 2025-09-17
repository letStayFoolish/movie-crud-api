using MovieApi.DTOs.Movie;

namespace MovieApi.Services.Movies;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto movie, CancellationToken cancellationToken = default);
    Task<MovieDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovieDto>> GetAllMoviesAsync(int take, int skip,CancellationToken cancellationToken = default);
    Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie, CancellationToken cancellationToken = default);
    Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default);
}
