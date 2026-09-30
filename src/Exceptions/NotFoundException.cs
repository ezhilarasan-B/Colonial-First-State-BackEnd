using System.Net;
using UserDirectory.Api.Constants;

namespace UserDirectory.Api.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.", HttpStatusCode.BadRequest, ErrorConstants.BadRequestErrorCode)
    {
    }

    public NotFoundException(string message) 
        : base(message, HttpStatusCode.BadRequest, ErrorConstants.BadRequestErrorCode)
    {
    }
}
