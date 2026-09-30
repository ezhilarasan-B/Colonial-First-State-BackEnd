using System.Net;
using FluentValidation.Results;
using UserDirectory.Api.Constants;

namespace UserDirectory.Api.Exceptions;

/// <summary>
/// Exception thrown when input validation fails.
/// Maps to HTTP 400 Bad Request.
/// </summary>
public class ValidationException : ApplicationException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message = "One or more validation failures have occurred.") 
        : base(message, HttpStatusCode.BadRequest, ErrorConstants.ValidationErrorCode)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string field, string error) 
        : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, ErrorConstants.ValidationErrorCode)
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, ErrorConstants.ValidationErrorCode)
    {
        Errors = errors;
    }

    public ValidationException(ValidationResult validationResult)
        : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, ErrorConstants.ValidationErrorCode)
    {
        Errors = validationResult.Errors
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, ErrorConstants.ValidationErrorCode)
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());
    }
}
