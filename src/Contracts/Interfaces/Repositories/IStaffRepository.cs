using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Models;

namespace UserDirectory.Api.Contracts.Interfaces.Repositories;

public interface IStaffRepository
{
    Task<PagedResult<StaffModel>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<StaffModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StaffModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StaffModel> CreateAsync(StaffModel staff, CancellationToken cancellationToken = default);
    Task<StaffModel?> UpdateAsync(StaffModel staff, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
}
