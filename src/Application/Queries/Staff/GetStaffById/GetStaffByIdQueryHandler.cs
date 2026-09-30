using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Queries.Staff.GetStaffById;

public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, StaffResponse?>
{
    private readonly IStaffService _staffService;

    public GetStaffByIdQueryHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<StaffResponse?> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
    {
        return await _staffService.GetStaffByIdAsync(request.Id, cancellationToken);
    }
}
