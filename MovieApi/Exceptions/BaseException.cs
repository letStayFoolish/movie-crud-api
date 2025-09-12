using System.Net;

namespace MovieApi.Exceptions;

// abstract - means that this class is meant to be a base class fot other class.
public abstract class BaseException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }

    public BaseException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) :
        base(message)
    {
        HttpStatusCode = httpStatusCode;
    }
}
