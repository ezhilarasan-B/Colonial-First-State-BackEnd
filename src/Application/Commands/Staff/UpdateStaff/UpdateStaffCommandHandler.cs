using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Staff.UpdateStaff;

public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, StaffResponse?>
{
    private readonly IStaffService _staffService;

    public UpdateStaffCommandHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<StaffResponse?> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        return await _staffService.UpdateStaffAsync(request.Id, request.Request, request.ModifiedBy, cancellationToken);
    }
}
