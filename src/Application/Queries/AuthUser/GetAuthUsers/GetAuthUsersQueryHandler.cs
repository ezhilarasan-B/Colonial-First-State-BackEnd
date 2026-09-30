using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Queries.AuthUser.GetAuthUsers;

public class GetAuthUsersQueryHandler : IRequestHandler<GetAuthUsersQuery, IEnumerable<AuthUserResponse>>
{
    private readonly IAuthUserService _authUserService;

    public GetAuthUsersQueryHandler(IAuthUserService authUserService)
    {
        _authUserService = authUserService;
    }

    public async Task<IEnumerable<AuthUserResponse>> Handle(GetAuthUsersQuery request, CancellationToken cancellationToken)
    {
        return await _authUserService.GetAllAsync(cancellationToken);
    }
}
