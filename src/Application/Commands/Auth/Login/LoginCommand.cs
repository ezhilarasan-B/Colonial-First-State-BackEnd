using MediatR;
using UserDirectory.Api.Contracts.Auth.Requests;
using UserDirectory.Api.Contracts.Auth.Responses;

namespace UserDirectory.Api.Application.Commands.Auth.Login;

/// <summary>
/// CQRS Command to authenticate a user with credentials.
/// </summary>
public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

