using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Queries.AuthUser.GetAuthUserById;

public class GetAuthUserByIdQueryHandler : IRequestHandler<GetAuthUserByIdQuery, AuthUserResponse?>
{
    private readonly IAuthUserService _authUserService;

    public GetAuthUserByIdQueryHandler(IAuthUserService authUserService)
    {
        _authUserService = authUserService;
    }

    public async Task<AuthUserResponse?> Handle(GetAuthUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _authUserService.GetByIdAsync(request.Id, cancellationToken);
    }
}
