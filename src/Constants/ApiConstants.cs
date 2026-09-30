namespace UserDirectory.Api.Constants;

public static class ApiConstants
{
    public const string CorsPolicyName = "AllowFrontendClient";
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string JsonContentType = "application/json";
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    /// <summary>
    /// Current API version prefix used in all route templates.
    /// Update this single value to change the version across all controllers.
    /// </summary>
    public const string ApiVersion = "v1";
    public const string RoutePrefix = "api/v1";
}
