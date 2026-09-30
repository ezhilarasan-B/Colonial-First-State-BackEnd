using Microsoft.Extensions.Logging;

namespace UserDirectory.Api.Infrastructure.Logging;

/// <summary>
/// Helper to create structured logging scopes enriched with correlation identifiers.
/// </summary>
public static class CorrelationLogScope
{
    public static IDisposable? Begin(ILogger logger, string correlationId)
    {
        return logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        });
    }
}

