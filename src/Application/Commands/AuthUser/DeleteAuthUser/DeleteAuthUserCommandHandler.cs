using MediatR;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.AuthUser.DeleteAuthUser;

public class DeleteAuthUserCommandHandler : IRequestHandler<DeleteAuthUserCommand, bool>
{
    private readonly IAuthUserService _authUserService;

    public DeleteAuthUserCommandHandler(IAuthUserService authUserService)
    {
        _authUserService = authUserService;
    }

    public async Task<bool> Handle(DeleteAuthUserCommand request, CancellationToken cancellationToken)
    {
        return await _authUserService.DeleteAsync(request.Id, request.DeletedBy, cancellationToken);
    }
}
