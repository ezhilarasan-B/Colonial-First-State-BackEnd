using FluentValidation;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Client.Models;
using UserDirectory.Api.Contracts.Client.Requests;
using UserDirectory.Api.Contracts.Client.Responses;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Services;
using ValidationException = UserDirectory.Api.Exceptions.ValidationException;

namespace UserDirectory.Api.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IValidator<CreateClientRequest> _createValidator;
    private readonly IValidator<UpdateClientRequest> _updateValidator;
    private readonly ILogger<ClientService> _logger;

    public ClientService(
        IClientRepository clientRepository,
        IStaffRepository staffRepository,
        IValidator<CreateClientRequest> createValidator,
        IValidator<UpdateClientRequest> updateValidator,
        ILogger<ClientService> logger)
    {
        _clientRepository = clientRepository;
        _staffRepository = staffRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<PagedResult<ClientResponse>> GetPagedClientsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var paged = await _clientRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        var responses = paged.Items.Select(MapToResponse).ToList();
        return new PagedResult<ClientResponse>(responses, paged.TotalCount, paged.PageNumber, paged.PageSize);
    }

    public async Task<IEnumerable<ClientResponse>> GetAllClientsAsync(CancellationToken cancellationToken = default)
    {
        var list = await _clientRepository.GetAllAsync(cancellationToken);
        return list.Select(MapToResponse);
    }

    public async Task<ClientResponse?> GetClientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = await _clientRepository.GetByIdAsync(id, cancellationToken);
        return client != null ? MapToResponse(client) : null;
    }

    public async Task<ClientResponse> CreateClientAsync(CreateClientRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        // Verify assigned staff exists
        var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff == null)
        {
            throw new NotFoundException("Staff", request.StaffId.ToString());
        }

        var client = new ClientModel
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Company = request.Company.Trim(),
            StaffId = request.StaffId,
            StaffName = staff.Name,
            CreatedDate = DateTime.UtcNow.ToString("o"),
            CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy
        };

        var created = await _clientRepository.CreateAsync(client, cancellationToken);
        _logger.LogInformation("Client created with ID {ClientId} assigned to Staff {StaffId}", created.Id, created.StaffId);
        return MapToResponse(created);
    }

    public async Task<ClientResponse?> UpdateClientAsync(int id, UpdateClientRequest request, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        var existing = await _clientRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return null;
        }

        // Verify assigned staff exists
        var staff = await _staffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff == null)
        {
            throw new NotFoundException("Staff", request.StaffId.ToString());
        }

        var now = DateTime.UtcNow.ToString("o");
        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        existing.Phone = request.Phone.Trim();
        existing.Company = request.Company.Trim();
        existing.StaffId = request.StaffId;
        existing.StaffName = staff.Name;
        existing.ModifiedDate = now;
        existing.ModifiedBy = string.IsNullOrWhiteSpace(modifiedBy) ? "System" : modifiedBy;
        existing.ModifiedOn = now;

        var updated = await _clientRepository.UpdateAsync(existing, cancellationToken);
        if (updated != null)
        {
            _logger.LogInformation("Client updated with ID {ClientId}", updated.Id);
            return MapToResponse(updated);
        }
        return null;
    }

    public async Task<bool> DeleteClientAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _clientRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        var success = await _clientRepository.DeleteAsync(id, string.IsNullOrWhiteSpace(deletedBy) ? "System" : deletedBy, cancellationToken);
        if (success)
        {
            _logger.LogInformation("Client soft-deleted with ID {ClientId} by {DeletedBy}", id, deletedBy);
        }
        return success;
    }

    public async Task<BulkDeleteClientResponse> BulkDeleteClientsAsync(List<int> ids, string deletedBy, CancellationToken cancellationToken = default)
    {
        var deletedIds = new List<int>();
        var user = string.IsNullOrWhiteSpace(deletedBy) ? "System" : deletedBy;
        foreach (var id in ids.Distinct())
        {
            var existing = await _clientRepository.GetByIdAsync(id, cancellationToken);
            if (existing != null)
            {
                var success = await _clientRepository.DeleteAsync(id, user, cancellationToken);
                if (success)
                {
                    deletedIds.Add(id);
                }
            }
        }
        _logger.LogInformation("Bulk soft-deleted {Count} clients by {DeletedBy}", deletedIds.Count, user);
        return new BulkDeleteClientResponse
        {
            DeletedIds = deletedIds,
            Message = $"Successfully deleted {deletedIds.Count} client(s)."
        };
    }

    private static ClientResponse MapToResponse(ClientModel model)
    {
        return new ClientResponse
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Company = model.Company,
            StaffId = model.StaffId,
            StaffName = model.StaffName,
            CreatedDate = model.CreatedDate,
            ModifiedDate = model.ModifiedDate
        };
    }
}
