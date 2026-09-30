using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;
using UserDirectory.Api.Contracts.Interfaces.Services;

namespace UserDirectory.Api.Application.Commands.Staff.BulkDeleteStaff;

public class BulkDeleteStaffCommandHandler : IRequestHandler<BulkDeleteStaffCommand, BulkDeleteStaffResponse>
{
    private readonly IStaffService _staffService;

    public BulkDeleteStaffCommandHandler(IStaffService staffService)
    {
        _staffService = staffService;
    }

    public async Task<BulkDeleteStaffResponse> Handle(BulkDeleteStaffCommand request, CancellationToken cancellationToken)
    {
        return await _staffService.BulkDeleteStaffAsync(request.Ids, request.DeletedBy, cancellationToken);
    }
}
