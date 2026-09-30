using MediatR;
using UserDirectory.Api.Contracts.Staff.Requests;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Application.Commands.Staff.CreateStaff;

public record CreateStaffCommand(CreateStaffRequest Request, string CreatedBy) : IRequest<StaffResponse>;
