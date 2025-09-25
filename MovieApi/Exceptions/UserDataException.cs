// This file is part of the project. Copyright (c) Company.

using System.Net;

namespace MovieApi.Exceptions;

public class UserDataException : BaseException
{
    public UserDataException(string? missingField) : base(
        $"User's field {nameof(missingField)} cannot be null or empty string.", HttpStatusCode.BadRequest)
    {
    }
}
