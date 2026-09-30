using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Application.Commands.Staff.BulkDeleteStaff;

public record BulkDeleteStaffCommand(List<int> Ids, string DeletedBy) : IRequest<BulkDeleteStaffResponse>;
