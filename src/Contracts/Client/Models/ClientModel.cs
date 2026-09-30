namespace UserDirectory.Api.Contracts.Client.Models;

public class ClientModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedOn { get; set; }
    public string? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedOn { get; set; }
}
