using MediatR;
using UserDirectory.Api.Contracts.AuthUser.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.AuthUser.CreateAuthUser;

public class CreateAuthUserCommandHandler : IRequestHandler<CreateAuthUserCommand, AuthUserResponse>
{
    private readonly IAuthUserService _authUserService;

    public CreateAuthUserCommandHandler(IAuthUserService authUserService)
    {
        _authUserService = authUserService;
    }

    public async Task<AuthUserResponse> Handle(CreateAuthUserCommand request, CancellationToken cancellationToken)
    {
        return await _authUserService.CreateAsync(request.Request, request.CreatedBy, cancellationToken);
    }
}
