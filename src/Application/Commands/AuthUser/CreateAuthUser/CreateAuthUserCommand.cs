using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Requests;
using UserDirectory.Api.Contracts.AuthUser.Responses;

namespace UserDirectory.Api.Application.Commands.AuthUser.CreateAuthUser;

public record CreateAuthUserCommand(CreateAuthUserRequest Request, string CreatedBy) : IRequest<AuthUserResponse>;
