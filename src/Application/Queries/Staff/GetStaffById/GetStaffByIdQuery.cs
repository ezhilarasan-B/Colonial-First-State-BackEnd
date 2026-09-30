using MediatR;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Application.Queries.Staff.GetStaffById;

public record GetStaffByIdQuery(int Id) : IRequest<StaffResponse?>;
