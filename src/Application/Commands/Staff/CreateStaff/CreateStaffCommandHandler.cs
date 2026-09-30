using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Staff.CreateStaff;

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, StaffResponse>
{
    private readonly IStaffService _staffService;

    public CreateStaffCommandHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<StaffResponse> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        return await _staffService.CreateStaffAsync(request.Request, request.CreatedBy, cancellationToken);
    }
}
