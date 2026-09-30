using UserDirectory.Api.Contracts.Client.Models;
using UserDirectory.Api.Contracts.Common;

namespace UserDirectory.Api.Contracts.Interfaces.Repositories;

public interface IClientRepository
{
    Task<PagedResult<ClientModel>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClientModel> CreateAsync(ClientModel client, CancellationToken cancellationToken = default);
    Task<ClientModel?> UpdateAsync(ClientModel client, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<bool> HasClientsAssignedToStaffAsync(int staffId, CancellationToken cancellationToken = default);
    Task<List<int>> GetAssignedStaffIdsAsync(IEnumerable<int> staffIds, CancellationToken cancellationToken = default);
}
