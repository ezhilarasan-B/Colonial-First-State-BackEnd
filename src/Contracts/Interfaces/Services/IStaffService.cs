using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Requests;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Contracts.Interfaces.Services;

public interface IStaffService
{
    Task<PagedResult<StaffResponse>> GetPagedStaffAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffResponse>> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<StaffResponse?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request, string createdBy, CancellationToken cancellationToken = default);
    Task<StaffResponse?> UpdateStaffAsync(int id, UpdateStaffRequest request, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<BulkDeleteStaffResponse> BulkDeleteStaffAsync(List<int> ids, string deletedBy, CancellationToken cancellationToken = default);
}
