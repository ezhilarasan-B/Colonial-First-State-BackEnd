using UserDirectory.Api.Contracts.Client.Requests;
using UserDirectory.Api.Contracts.Client.Responses;
using UserDirectory.Api.Contracts.Common;

namespace UserDirectory.Api.Contracts.Interfaces.Services;

public interface IClientService
{
    Task<PagedResult<ClientResponse>> GetPagedClientsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientResponse>> GetAllClientsAsync(CancellationToken cancellationToken = default);
    Task<ClientResponse?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ClientResponse> CreateClientAsync(CreateClientRequest request, string createdBy, CancellationToken cancellationToken = default);
    Task<ClientResponse?> UpdateClientAsync(int id, UpdateClientRequest request, string modifiedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteClientAsync(int id, string deletedBy, CancellationToken cancellationToken = default);
    Task<BulkDeleteClientResponse> BulkDeleteClientsAsync(List<int> ids, string deletedBy, CancellationToken cancellationToken = default);
}
