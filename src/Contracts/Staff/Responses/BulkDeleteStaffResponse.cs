namespace UserDirectory.Api.Contracts.Staff.Responses;

/// <summary>
/// Response payload for staff bulk delete operations, indicating deleted records
/// and reporting staff records that could not be deleted due to assigned clients.
/// </summary>
public class BulkDeleteStaffResponse
{
    public List<int> DeletedIds { get; set; } = new();
    public List<StaffAssignedClientFailure> FailedStaff { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class StaffAssignedClientFailure
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = "Staff was assigned to client, we can't delete.";
}
