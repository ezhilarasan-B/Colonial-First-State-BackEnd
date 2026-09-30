using MediatR;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Contracts.Staff.Responses;

namespace UserDirectory.Api.Application.Queries.Staff.GetStaff;

/// <summary>
/// CQRS Query to retrieve paginated active (non-deleted) staff.
/// </summary>
public record GetStaffQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<StaffResponse>>;
