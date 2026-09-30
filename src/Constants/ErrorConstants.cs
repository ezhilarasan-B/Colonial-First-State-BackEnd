namespace UserDirectory.Api.Constants;

public static class ErrorConstants
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string ValidationErrorCode = "VALIDATION_ERROR";

    public const string NotFoundError = "NOT_FOUND";
    public const string NotFoundErrorCode = "NOT_FOUND";

    public const string UnauthorizedError = "UNAUTHORIZED";
    public const string UnauthorizedCode = "UNAUTHORIZED";

    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string InternalServerErrorCode = "INTERNAL_SERVER_ERROR";

    public const string InvalidCredentials = "Invalid username or password.";
    public const string InvalidCredentialsError = "INVALID_CREDENTIALS";

    public const string TokenExpiredError = "TOKEN_EXPIRED";

    public const string BadRequest = "BAD_REQUEST";
    public const string BadRequestErrorCode = "BAD_REQUEST";
}
