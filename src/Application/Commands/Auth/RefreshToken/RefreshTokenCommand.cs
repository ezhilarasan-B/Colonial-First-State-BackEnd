using MediatR;
using UserDirectory.Api.Contracts.Auth.Requests;
using UserDirectory.Api.Contracts.Auth.Responses;

namespace UserDirectory.Api.Application.Commands.Auth.RefreshToken;

/// <summary>
/// CQRS Command to rotate a refresh token and issue a fresh access token.
/// </summary>
public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<RefreshTokenResponse>;

