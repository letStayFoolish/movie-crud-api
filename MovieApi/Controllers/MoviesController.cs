using Microsoft.AspNetCore.Mvc;
using MovieApi.DTOs.Movie;
using MovieApi.Filters;
using MovieApi.Services.Movies;

namespace MovieApi.Controllers;

[ApiController]
// [TypeFilter(typeof(LoggingFilter))]
[TypeFilter(typeof(ValidateModelFilter))]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;

    public MoviesController(IMovieService service)
    {
        _service = service;
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto command, CancellationToken cancellationToken)
    {
        var movie = await _service.CreateMovieAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetMovieById), new { Id = movie.Id }, movie);
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> GetAllMovies(CancellationToken cancellationToken)
    {
        var movies = await _service.GetAllMoviesAsync(cancellationToken);
        return Ok(movies);
    }

    // GET
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetMovieById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var movie = await _service.GetMovieByIdAsync(id, cancellationToken);

        if (movie is null)
        {
            return NotFound(new { Message = $"Movie with ID {id} not found." });
        }

        return Ok(movie);
    }

    // PUT
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] UpdateMovieDto command,
        CancellationToken cancellationToken)
    {
        await _service.UpdateMovieAsync(id, command, cancellationToken);
        return NoContent();
    }

    // DELETE
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _service.DeleteMovieAsync(id, cancellationToken);
        return NoContent();
    }
}
