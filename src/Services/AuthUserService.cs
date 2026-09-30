using FluentValidation;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Contracts.Auth.Models;
using UserDirectory.Api.Contracts.AuthUser.Requests;
using UserDirectory.Api.Contracts.AuthUser.Responses;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Services;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Infrastructure.Authentication;
using ValidationException = UserDirectory.Api.Exceptions.ValidationException;
using AppException = UserDirectory.Api.Exceptions.ApplicationException;

namespace UserDirectory.Api.Services;

/// <summary>
/// Service for managing AuthUser accounts (create, read, delete).
/// Separate from AuthService which handles JWT login/refresh only.
/// </summary>
public class AuthUserService : IAuthUserService
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<CreateAuthUserRequest> _createValidator;
    private readonly ILogger<AuthUserService> _logger;

    public AuthUserService(
        IAuthUserRepository authUserRepository,
        IJwtTokenService jwtTokenService,
        IValidator<CreateAuthUserRequest> createValidator,
        ILogger<AuthUserService> logger)
    {
        _authUserRepository = authUserRepository;
        _jwtTokenService = jwtTokenService;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<AuthUserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _authUserRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToResponse);
    }

    public async Task<AuthUserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _authUserRepository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<AuthUserResponse> CreateAsync(
        CreateAuthUserRequest request,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult);
        }

        // Check if username already exists
        var existing = await _authUserRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (existing != null)
        {
            throw new AppException($"Username '{request.Username}' is already taken.");
        }

        var salt = _jwtTokenService.GenerateSalt();
        var hash = _jwtTokenService.HashPassword(request.Password, salt);

        var authUser = new AuthUserModel
        {
            Username = request.Username.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Email = request.Email.Trim(),
            Role = request.Role.Trim(),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy,
        };

        var created = await _authUserRepository.CreateAsync(authUser, cancellationToken);
        _logger.LogInformation("AuthUser '{Username}' created by '{CreatedBy}'.", created.Username, createdBy);

        return MapToResponse(created);
    }

    public async Task<bool> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var user = await _authUserRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("AuthUser", id.ToString());
        }

        var deleted = await _authUserRepository.DeleteAsync(id, deletedBy, cancellationToken);
        if (deleted)
        {
            _logger.LogInformation("AuthUser ID {Id} soft-deleted by '{DeletedBy}'.", id, deletedBy);
        }

        return deleted;
    }

    private static AuthUserResponse MapToResponse(AuthUserModel model) => new()
    {
        Id = model.Id,
        Username = model.Username,
        Email = model.Email,
        Role = model.Role,
        CreatedDate = model.CreatedDate,
        CreatedBy = model.CreatedBy,
    };
}
