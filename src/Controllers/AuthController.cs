using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserDirectory.Api.Application.Commands.Auth.Login;
using UserDirectory.Api.Application.Commands.Auth.RefreshToken;
using UserDirectory.Api.Contracts.Auth.Requests;
using UserDirectory.Api.Contracts.Auth.Responses;
using UserDirectory.Api.Contracts.Common;

namespace UserDirectory.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return Ok(response);
    }
}
