using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MovieApi.DTOs.Movie;
using MovieApi.Exceptions;
using MovieApi.Models;
using MovieApi.Persistence;

namespace MovieApi.Services.Movies;

public sealed class MovieService : IMovieService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MovieService> _logger;

    private const string MoviesCacheKey = "movies";

    public MovieService(ApplicationDbContext context, IMemoryCache cache, ILogger<MovieService> logger)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Asynchronously creates a new movie and saves it to the database.
    /// </summary>
    /// <param name="movieDto">The data transfer object containing information about the movie to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the newly created movie as a data transfer object.</returns>
    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto movieDto, CancellationToken cancellationToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Creating movie with title: {title}, info: {@movie}.", movieDto.Title, movieDto);
        }

        var newMovie = Movie.Create(movieDto.Title, movieDto.Genre, movieDto.ReleaseDate, movieDto.Rating);
        var cacheKey = MoviesCacheKey;
        _logger.LogInformation("invalidating cache for key: {CacheKey} from cache.", cacheKey);
        _cache.Remove(cacheKey);
        await _context.Movies.AddAsync(newMovie, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new MovieDto(newMovie.Id, newMovie.Title, newMovie.Genre, newMovie.ReleaseDate, newMovie.Rating);
    }

    /// <summary>
    /// Asynchronously retrieves a movie by its unique identifier.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the movie data transfer object if found; otherwise, null.</returns>
    public async Task<MovieDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"movie:{movieId}";
        _logger.LogInformation("fetching data for key: {CacheKey} from cache.", cacheKey);
        if (!_cache.TryGetValue(cacheKey, out MovieDto? movieDto))
        {
            _logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);


            _logger.LogInformation("setting data for key: {CacheKey} to cache.", cacheKey);
            var cacheSettings = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                .SetPriority(CacheItemPriority.Normal);
            _cache.Set(cacheKey, movieDto, cacheSettings);;
        }

        var movieFound = await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(movie => movie.Id == movieId, cancellationToken);

        if (movieFound is null)
        {
            throw new MovieNotFoundException(movieId);
        }

        return new MovieDto(movieFound.Id, movieFound.Title, movieFound.Genre, movieFound.ReleaseDate,
            movieFound.Rating);
    }

    public sealed record PageResult<T>(
        IReadOnlyList<T> Items,
        long TotalCount,
        int Page,
        int PageSize,
        int TotalPages
    );

    /// <summary>
    /// Asynchronously retrieves a paginated list of movies from the database or cache.
    /// </summary>
    /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
    /// <param name="pageSize">The number of items per page. Must be greater than 0.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated result of movies, including the total count and metadata for pagination.</returns>
    public async Task<PageResult<MovieDto>> GetAllMoviesAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        // page = Math.Max(1, page);
        // pageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
        var cacheKey = MoviesCacheKey;
        // var cacheKey = $"{MoviesCacheKey}:{page}:{pageSize}";
        _logger.LogInformation("fetching data for key: {CacheKey} from cache.", cacheKey);

        if (!_cache.TryGetValue(cacheKey, out PageResult<MovieDto>? pageResult))
        {
            _logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database.", cacheKey);

            var baseQuery = _context.Movies.AsNoTracking();

            var totalCount = await baseQuery.LongCountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(m => m.Title)
                .ThenBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDto(
                    m.Id,
                    m.Title,
                    m.Genre,
                    m.ReleaseDate,
                    m.Rating
                )).ToListAsync(cancellationToken);

            int totalPages = (int)Math.Ceiling(totalCount / (double) pageSize);

            pageResult = new PageResult<MovieDto>(items, totalCount, page, pageSize, totalPages);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                .SetPriority(CacheItemPriority.NeverRemove)
                .SetSize(2048);
            _logger.LogInformation("setting data for key: {CacheKey} to cache.", cacheKey);
            _cache.Set(cacheKey, pageResult, cacheOptions);
        }

        return pageResult;
    }

    /// <summary>
    /// Asynchronously updates the details of an existing movie in the database.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie to be updated.</param>
    /// <param name="movie">The data transfer object containing the updated details of the movie.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie,
        CancellationToken cancellationToken = default)
    {
        var movieToUpdate = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == movieId, cancellationToken);

        if (movieToUpdate is null)
        {
            // throw new ArgumentNullException($"Invalid movie id: {movieId}.");
            throw new MovieNotFoundException(movieId);
        }

        var newTitle = movie.Title ?? movieToUpdate.Title;
        var newGenre = movie.Genre ?? movieToUpdate.Genre;
        var newReleaseDate = movie.ReleaseDate ?? movieToUpdate.ReleaseDate;
        var newRating = movie.Rating ?? movieToUpdate.Rating;

        var cacheKey = MoviesCacheKey;
        _cache.Remove(cacheKey);
        movieToUpdate.Update(newTitle, newGenre, newReleaseDate, newRating);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously deletes a movie from the database if it exists.
    /// </summary>
    /// <param name="movieId">The unique identifier of the movie to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        var movieToDelete = await _context.Movies
            .FirstOrDefaultAsync(movie => movie.Id == movieId, cancellationToken);

        if (movieToDelete is null)
        {
            throw new MovieNotFoundException(movieId);
        }

        var cacheKey = MoviesCacheKey;
        _cache.Remove(cacheKey);
        _context.Movies.Remove(movieToDelete);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
