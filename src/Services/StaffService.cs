using FluentValidation;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Models;
using UserDirectory.Api.Contracts.Staff.Requests;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Services;
using ValidationException = UserDirectory.Api.Exceptions.ValidationException;

namespace UserDirectory.Api.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<CreateStaffRequest> _createValidator;
    private readonly IValidator<UpdateStaffRequest> _updateValidator;
    private readonly ILogger<StaffService> _logger;

    public StaffService(
        IStaffRepository staffRepository,
        IClientRepository clientRepository,
        IValidator<CreateStaffRequest> createValidator,
        IValidator<UpdateStaffRequest> updateValidator,
        ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _clientRepository = clientRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<PagedResult<StaffResponse>> GetPagedStaffAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var pagedModels = await _staffRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        var responses = pagedModels.Items.Select(MapToResponse).ToList();
        return new PagedResult<StaffResponse>(responses, pagedModels.TotalCount, pagedModels.PageNumber, pagedModels.PageSize);
    }

    public async Task<IEnumerable<StaffResponse>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        var staffList = await _staffRepository.GetAllAsync(cancellationToken);
        return staffList.Select(MapToResponse);
    }

    public async Task<StaffResponse?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
        return staff != null ? MapToResponse(staff) : null;
    }

    public async Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        var staff = new StaffModel
        {
            Name = request.Name.Trim(),
            Age = request.Age,
            City = request.City.Trim(),
            State = request.State.Trim(),
            Pincode = request.Pincode.Trim(),
            CreatedDate = DateTime.UtcNow.ToString("o"),
            CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy
        };

        var created = await _staffRepository.CreateAsync(staff, cancellationToken);
        _logger.LogInformation("Staff created with ID {StaffId} by {CreatedBy}", created.Id, created.CreatedBy);
        return MapToResponse(created);
    }

    public async Task<StaffResponse?> UpdateStaffAsync(int id, UpdateStaffRequest request, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult);
        }

        var existing = await _staffRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return null;
        }

        var now = DateTime.UtcNow.ToString("o");
        existing.Name = request.Name.Trim();
        existing.Age = request.Age;
        existing.City = request.City.Trim();
        existing.State = request.State.Trim();
        existing.Pincode = request.Pincode.Trim();
        existing.ModifiedDate = now;
        existing.ModifiedBy = string.IsNullOrWhiteSpace(modifiedBy) ? "System" : modifiedBy;
        existing.ModifiedOn = now;

        var updated = await _staffRepository.UpdateAsync(existing, cancellationToken);
        if (updated != null)
        {
            _logger.LogInformation("Staff updated with ID {StaffId} by {ModifiedBy}", updated.Id, updated.ModifiedBy);
            return MapToResponse(updated);
        }
        return null;
    }

    public async Task<bool> DeleteStaffAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _staffRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        var isAssigned = await _clientRepository.HasClientsAssignedToStaffAsync(id, cancellationToken);
        if (isAssigned)
        {
            _logger.LogWarning("Cannot delete staff with ID {StaffId} because they are assigned to one or more clients.", id);
            throw new ValidationException("Staff was assigned to client, we can't delete.");
        }

        var success = await _staffRepository.DeleteAsync(id, string.IsNullOrWhiteSpace(deletedBy) ? "System" : deletedBy, cancellationToken);
        if (success)
        {
            _logger.LogInformation("Staff soft-deleted with ID {StaffId} by {DeletedBy}", id, deletedBy);
        }
        return success;
    }

    public async Task<BulkDeleteStaffResponse> BulkDeleteStaffAsync(List<int> ids, string deletedBy, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        var assignedStaffIds = await _clientRepository.GetAssignedStaffIdsAsync(distinctIds, cancellationToken);
        var assignedSet = new HashSet<int>(assignedStaffIds);

        var deletedIds = new List<int>();
        var failedStaff = new List<StaffAssignedClientFailure>();
        var user = string.IsNullOrWhiteSpace(deletedBy) ? "System" : deletedBy;

        foreach (var id in distinctIds)
        {
            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            if (staff == null) continue;

            if (assignedSet.Contains(id))
            {
                _logger.LogWarning("Staff with ID {StaffId} ({Name}) is assigned to a client and cannot be deleted.", id, staff.Name);
                failedStaff.Add(new StaffAssignedClientFailure
                {
                    Id = id,
                    Name = staff.Name,
                    Reason = "Staff was assigned to client, we can't delete."
                });
            }
            else
            {
                var success = await _staffRepository.DeleteAsync(id, user, cancellationToken);
                if (success)
                {
                    deletedIds.Add(id);
                }
            }
        }

        string message;
        if (failedStaff.Count > 0 && deletedIds.Count == 0)
        {
            message = "Staff was assigned to client, we can't delete.";
        }
        else if (failedStaff.Count > 0 && deletedIds.Count > 0)
        {
            message = $"Deleted {deletedIds.Count} staff member(s). Staff was assigned to client, we can't delete: {string.Join(", ", failedStaff.Select(s => s.Name))}";
        }
        else
        {
            message = $"Successfully deleted {deletedIds.Count} staff member(s).";
        }

        return new BulkDeleteStaffResponse
        {
            DeletedIds = deletedIds,
            FailedStaff = failedStaff,
            Message = message
        };
    }

    private static StaffResponse MapToResponse(StaffModel model)
    {
        return new StaffResponse
        {
            Id = model.Id,
            Name = model.Name,
            Age = model.Age,
            City = model.City,
            State = model.State,
            Pincode = model.Pincode,
            CreatedDate = model.CreatedDate,
            ModifiedDate = model.ModifiedDate
        };
    }
}
