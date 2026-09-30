namespace UserDirectory.Api.Contracts.Client.Responses;

/// <summary>
/// Response payload for client bulk delete operations.
/// </summary>
public class BulkDeleteClientResponse
{
    public List<int> DeletedIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
