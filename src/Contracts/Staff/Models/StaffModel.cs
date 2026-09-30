namespace UserDirectory.Api.Contracts.Staff.Models;

public class StaffModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedOn { get; set; }
    public string? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletedOn { get; set; }
}
