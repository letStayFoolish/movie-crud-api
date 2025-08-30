using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Movie;
using MovieApi.Models;
using MovieApi.Persistence;

namespace MovieApi.Services;

public class MovieService : IMovieService
{
    private readonly MovieDbContext _context;

    public MovieService(MovieDbContext context)
    {
        _context = context;
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto movieDto)
    {
        var newMovie = Movie.Create(movieDto.Title, movieDto.Genre, movieDto.ReleaseDate, movieDto.Rating);
        await _context.Movies.AddAsync(newMovie);
        await _context.SaveChangesAsync();
        return new MovieDto(newMovie.Id, newMovie.Title, newMovie.Genre, newMovie.ReleaseDate, newMovie.Rating);
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid movieId)
    {
        var movieFound = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(movie => movie.Id == movieId);
        return movieFound is null
            ? null
            : new MovieDto(movieFound.Id, movieFound.Title, movieFound.Genre, movieFound.ReleaseDate,
                movieFound.Rating);
    }

    public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
    {
        // retrieves all movies from the database without tracking changes for better performance.
        return await _context.Movies.AsNoTracking().Select(m => new MovieDto(
            m.Id,
            m.Title,
            m.Genre,
            m.ReleaseDate,
            m.Rating
        )).ToListAsync();
    }

    public async Task UpdateMovieAsync(Guid movieId, UpdateMovieDto movie)
    {
        var movieToUpdate = await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == movieId);
        if (movieToUpdate is null)
        {
            throw new ArgumentNullException($"Invalid movie id: {movieId}.");
        }

        movieToUpdate.Update(movie.Title, movie.Genre, movie.ReleaseDate, movie.Rating);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(Guid movieId)
    {
        var movieToDelete = await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == movieId);
        if (movieToDelete is not null)
        {
            _context.Movies.Remove(movieToDelete);
            await _context.SaveChangesAsync();
        }
    }
}