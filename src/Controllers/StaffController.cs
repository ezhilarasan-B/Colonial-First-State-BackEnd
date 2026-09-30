using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserDirectory.Api.Application.Commands.Staff.CreateStaff;
using UserDirectory.Api.Application.Commands.Staff.DeleteStaff;
using UserDirectory.Api.Application.Commands.Staff.BulkDeleteStaff;
using UserDirectory.Api.Application.Commands.Staff.UpdateStaff;
using UserDirectory.Api.Application.Queries.Staff.GetStaff;
using UserDirectory.Api.Application.Queries.Staff.GetStaffById;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Requests;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // /api/v1/staff
[Authorize]
[Tags("Staff")]
public class StaffController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStaffService _staffService;

    public StaffController(IMediator mediator, IStaffService staffService)
    {
        _mediator = mediator;
        _staffService = staffService;
    }

    /// <summary>
    /// Retrieves paginated active staff directory entries (default 10 per page).
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "StaffRead")]
    [ProducesResponseType(typeof(PagedResult<StaffResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        var result = await _mediator.Send(new GetStaffQuery(pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all active staff without pagination (ideal for dropdown selection).
    /// </summary>
    [HttpGet("all")]
    [Authorize(Policy = "StaffRead")]
    [ProducesResponseType(typeof(IEnumerable<StaffResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUnpaginated(CancellationToken cancellationToken = default)
    {
        var staffList = await _staffService.GetAllStaffAsync(cancellationToken);
        return Ok(staffList);
    }

    /// <summary>
    /// Retrieves a staff member by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "StaffRead")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var staff = await _mediator.Send(new GetStaffByIdQuery(id), cancellationToken);
        if (staff == null)
        {
            throw new NotFoundException("Staff", id.ToString());
        }
        return Ok(staff);
    }

    /// <summary>
    /// Creates a new staff directory member.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "StaffWrite")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var created = await _mediator.Send(new CreateStaffCommand(request, currentUser), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing staff member.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "StaffWrite")]
    [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStaffRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var updated = await _mediator.Send(new UpdateStaffCommand(id, request, currentUser), cancellationToken);
        if (updated == null)
        {
            throw new NotFoundException("Staff", id.ToString());
        }
        return Ok(updated);
    }

    /// <summary>
    /// Soft deletes a staff member.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "StaffWrite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var success = await _mediator.Send(new DeleteStaffCommand(id, currentUser), cancellationToken);
        if (!success)
        {
            throw new NotFoundException("Staff", id.ToString());
        }
        return NoContent();
    }

    /// <summary>
    /// Bulk soft deletes staff members.
    /// Staff members assigned to clients cannot be deleted.
    /// </summary>
    [HttpPost("bulk-delete")]
    [Authorize(Policy = "StaffWrite")]
    [ProducesResponseType(typeof(BulkDeleteStaffResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var result = await _mediator.Send(new BulkDeleteStaffCommand(request.Ids, currentUser), cancellationToken);
        if (result.FailedStaff.Count > 0 && result.DeletedIds.Count == 0)
        {
            return BadRequest(new ErrorResponse
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Staff was assigned to client, we can't delete.",
                ErrorCode = "VAL_STAFF_ASSIGNED_TO_CLIENT"
            });
        }
        return Ok(result);
    }
}
