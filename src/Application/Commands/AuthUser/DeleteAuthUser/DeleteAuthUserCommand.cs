using MediatR;

namespace UserDirectory.Api.Application.Commands.AuthUser.DeleteAuthUser;

public record DeleteAuthUserCommand(int Id, string DeletedBy) : IRequest<bool>;
