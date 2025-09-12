using System.Net;

namespace MovieApi.Exceptions;

public class MovieNotFoundException : BaseException
{
    public MovieNotFoundException(Guid id) : base($"Movie with id {id}, not found.", HttpStatusCode.NotFound)
    {
    }
}
