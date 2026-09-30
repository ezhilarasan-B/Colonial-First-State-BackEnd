using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Responses;

namespace UserDirectory.Api.Application.Queries.AuthUser.GetAuthUserById;

public record GetAuthUserByIdQuery(int Id) : IRequest<AuthUserResponse?>;
