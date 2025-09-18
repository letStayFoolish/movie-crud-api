using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieApi.DTOs.Movie;
using MovieApi.Filters;
using MovieApi.Models;
using MovieApi.Services.Movies;

namespace MovieApi.Controllers;

[ApiController]
// [TypeFilter(typeof(LoggingFilter))]
// [TypeFilter(typeof(ValidateModelFilter))] // TODO: for this to be working we need to ad in Program.cs: builder.Services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true); // let your ValidateModelFilter handle invalid ModelState
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;
    private readonly PaginationOptions _paginationOptions;
    // Sensible defaults for pagination
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;


    public MoviesController(IMovieService service, IOptions<PaginationOptions> paginationOptions)
    {
        _service = service;
        _paginationOptions = paginationOptions.Value;
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
    public async Task<IActionResult> GetAllMovies(CancellationToken cancellationToken, [FromQuery] int? currentPage, [FromQuery] int? pageSize)
    {
        var effectivePageNumber = Math.Max(0, currentPage ?? _paginationOptions.Page);
        var effectivePageSize = pageSize is >= 1
            ? Math.Min(pageSize.Value, _paginationOptions.MaxPageSize)
            : _paginationOptions.PageSize;

        var movies = await _service.GetAllMoviesAsync(effectivePageNumber, effectivePageSize, cancellationToken);
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
