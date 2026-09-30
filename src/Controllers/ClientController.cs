using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserDirectory.Api.Contracts.Client.Requests;
using UserDirectory.Api.Contracts.Client.Responses;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Exceptions;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // /api/v1/client
[Authorize]
[Tags("Client")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Retrieves paginated clients (default 10 per page) with assigned staff details.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "ClientRead")]
    [ProducesResponseType(typeof(PagedResult<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        var result = await _clientService.GetPagedClientsAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a client by integer ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "ClientRead")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var client = await _clientService.GetClientByIdAsync(id, cancellationToken);
        if (client == null)
        {
            throw new NotFoundException("Client", id.ToString());
        }
        return Ok(client);
    }

    /// <summary>
    /// Creates a new client and assigns them to an existing staff member.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ClientWrite")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var created = await _clientService.CreateClientAsync(request, currentUser, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing client and their assigned staff member.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = "ClientWrite")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var updated = await _clientService.UpdateClientAsync(id, request, currentUser, cancellationToken);
        if (updated == null)
        {
            throw new NotFoundException("Client", id.ToString());
        }
        return Ok(updated);
    }

    /// <summary>
    /// Soft deletes a client.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ClientWrite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var success = await _clientService.DeleteClientAsync(id, currentUser, cancellationToken);
        if (!success)
        {
            throw new NotFoundException("Client", id.ToString());
        }
        return NoContent();
    }

    /// <summary>
    /// Bulk soft deletes clients.
    /// </summary>
    [HttpPost("bulk-delete")]
    [Authorize(Policy = "ClientWrite")]
    [ProducesResponseType(typeof(BulkDeleteClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request, CancellationToken cancellationToken = default)
    {
        var currentUser = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? "System";
        var result = await _clientService.BulkDeleteClientsAsync(request.Ids, currentUser, cancellationToken);
        return Ok(result);
    }
}
