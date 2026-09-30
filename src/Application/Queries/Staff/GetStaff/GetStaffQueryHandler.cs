using MediatR;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Queries.Staff.GetStaff;

public class GetStaffQueryHandler : IRequestHandler<GetStaffQuery, PagedResult<StaffResponse>>
{
    private readonly IStaffService _staffService;

    public GetStaffQueryHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<PagedResult<StaffResponse>> Handle(GetStaffQuery request, CancellationToken cancellationToken)
    {
        return await _staffService.GetPagedStaffAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
