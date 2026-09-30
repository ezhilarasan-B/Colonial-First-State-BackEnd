using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserDirectory.Api.Application.Commands.AuthUser.CreateAuthUser;
using UserDirectory.Api.Application.Commands.AuthUser.DeleteAuthUser;
using UserDirectory.Api.Application.Queries.AuthUser.GetAuthUsers;
using UserDirectory.Api.Application.Queries.AuthUser.GetAuthUserById;
using UserDirectory.Api.Contracts.AuthUser.Requests;
using UserDirectory.Api.Contracts.AuthUser.Responses;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Exceptions;

namespace UserDirectory.Api.Controllers;

/// <summary>
/// Manages authentication users (admin-only CRUD).
/// Separate from AuthController which handles login/refresh token operations only.
/// </summary>
[ApiController]
[Route("api/v1/authusers")]
[Authorize]
[Tags("Auth Users")]
public class AuthUserController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthUserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all active authentication users.
    /// Admin only.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AuthUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuthUsersQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single authentication user by ID.
    /// Admin only.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAuthUserByIdQuery(id), cancellationToken);
        if (result == null)
        {
            throw new NotFoundException("AuthUser", id.ToString());
        }
        return Ok(result);
    }

    /// <summary>
    /// Creates a new authentication user.
    /// Admin only.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateAuthUserRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name)
                          ?? User.FindFirstValue("unique_name")
                          ?? "System";

        var created = await _mediator.Send(new CreateAuthUserCommand(request, currentUser), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Soft-deletes an authentication user by ID.
    /// Admin only. You cannot delete your own account.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name)
                          ?? User.FindFirstValue("unique_name")
                          ?? "System";

        // Prevent self-deletion
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue("sub");
        if (int.TryParse(currentUserId, out var currentId) && currentId == id)
        {
            return BadRequest(new ErrorResponse
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "You cannot delete your own account.",
                ErrorCode = "SELF_DELETE_NOT_ALLOWED"
            });
        }

        var success = await _mediator.Send(new DeleteAuthUserCommand(id, currentUser), cancellationToken);
        if (!success)
        {
            throw new NotFoundException("AuthUser", id.ToString());
        }
        return NoContent();
    }
}
