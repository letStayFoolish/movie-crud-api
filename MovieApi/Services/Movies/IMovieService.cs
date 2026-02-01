using MovieApi.DTOs.Movie;

namespace MovieApi.Services.Movies;

public interface IMovieService
{
    public Task<MovieDto> CreateMovieAsync(CreateMovieDto movie, CancellationToken cancellationToken = default);
    public Task<MovieDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default);
    public Task<MovieService.PageResult<MovieDto>> GetAllMoviesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    public Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie, CancellationToken cancellationToken = default);
    public Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default);
}
