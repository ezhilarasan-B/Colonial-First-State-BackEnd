using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Responses;

namespace UserDirectory.Api.Application.Queries.AuthUser.GetAuthUsers;

public record GetAuthUsersQuery : IRequest<IEnumerable<AuthUserResponse>>;
