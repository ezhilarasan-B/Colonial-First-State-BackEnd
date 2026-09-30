using System.Net;
using UserDirectory.Api.Constants;

namespace UserDirectory.Api.Exceptions;

/// <summary>
/// Base exception type for all custom domain and application exceptions.
/// </summary>
public class ApplicationException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }

    public ApplicationException(
        string message, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest, 
        string errorCode = ErrorConstants.BadRequestErrorCode,
        Exception? innerException = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

