namespace UserDirectory.Api.Contracts.Client.Responses;

public class ClientResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string? ModifiedDate { get; set; }

    public string CreatedAt => CreatedDate;
    public string? UpdatedAt => ModifiedDate;
}
