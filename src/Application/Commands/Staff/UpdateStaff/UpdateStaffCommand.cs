using MediatR;
using UserDirectory.Api.Contracts.Staff.Requests;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Application.Commands.Staff.UpdateStaff;

public record UpdateStaffCommand(int Id, UpdateStaffRequest Request, string ModifiedBy) : IRequest<StaffResponse?>;
