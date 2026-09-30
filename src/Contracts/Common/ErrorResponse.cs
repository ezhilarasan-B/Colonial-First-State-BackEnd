namespace UserDirectory.Api.Contracts.Common;

/// <summary>
/// Consistent error contract returned to API consumers upon error conditions.
/// </summary>
public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

