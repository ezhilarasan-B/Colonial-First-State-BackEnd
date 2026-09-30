namespace UserDirectory.Api.Contracts.Common;

/// <summary>
/// Contract for bulk deletion of records by ID list.
/// </summary>
public class BulkDeleteRequest
{
    public List<int> Ids { get; set; } = new();
}
