namespace UserDirectory.Api.Contracts.Staff.Responses;

public class StaffResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string? ModifiedDate { get; set; }

    // Compatibility aliases
    public string CreatedAt => CreatedDate;
    public string? UpdatedAt => ModifiedDate;
}
