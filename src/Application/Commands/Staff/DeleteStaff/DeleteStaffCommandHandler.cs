using MediatR;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Staff.DeleteStaff;

public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand, bool>
{
    private readonly IStaffService _staffService;

    public DeleteStaffCommandHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<bool> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
    {
        return await _staffService.DeleteStaffAsync(request.Id, request.DeletedBy, cancellationToken);
    }
}
