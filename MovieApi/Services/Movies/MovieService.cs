using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Movie;
using MovieApi.Exceptions;
using MovieApi.Models;
using MovieApi.Persistence;

namespace MovieApi.Services.Movies;

public sealed class MovieService : IMovieService
{
    private readonly MovieDbContext _context;
    private readonly ILogger<MovieService> _logger;

    public MovieService(MovieDbContext context, ILogger<MovieService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto movieDto, CancellationToken cancellationToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Creating movie with title: {title}, info: {@movie}.", movieDto.Title, movieDto);
        }

        var newMovie = Movie.Create(movieDto.Title, movieDto.Genre, movieDto.ReleaseDate, movieDto.Rating);
        await _context.Movies.AddAsync(newMovie, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new MovieDto(newMovie.Id, newMovie.Title, newMovie.Genre, newMovie.ReleaseDate, newMovie.Rating);
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
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

    public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync(CancellationToken cancellationToken = default)
    {
        // retrieves all movies from the database without tracking changes for better performance.
        return await _context.Movies
            .AsNoTracking()
            .Select(m => new MovieDto(
                m.Id,
                m.Title,
                m.Genre,
                m.ReleaseDate,
                m.Rating
            )).ToListAsync(cancellationToken);
    }

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

        movieToUpdate.Update(movie.Title, movie.Genre, movie.ReleaseDate, movie.Rating);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMovieAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        var movieToDelete = await _context.Movies
            .FirstOrDefaultAsync(movie => movie.Id == movieId, cancellationToken);

        if (movieToDelete is null)
        {
            throw new MovieNotFoundException(movieId);
        }

        _context.Movies.Remove(movieToDelete);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
