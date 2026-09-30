using MediatR;

namespace UserDirectory.Api.Application.Commands.Staff.DeleteStaff;

public record DeleteStaffCommand(int Id, string DeletedBy) : IRequest<bool>;
