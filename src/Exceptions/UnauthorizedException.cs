using System.Net;
using UserDirectory.Api.Constants;

namespace UserDirectory.Api.Exceptions;

/// <summary>
/// Exception thrown when authentication or token validation fails.
/// Maps to HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = ErrorConstants.InvalidCredentials) 
        : base(message, HttpStatusCode.Unauthorized, ErrorConstants.UnauthorizedCode)
    {
    }
}

