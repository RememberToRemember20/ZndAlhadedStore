using AuthKit.Common;
using AuthKit.Models;
using MediatR;

namespace ZndAlhadedStore.Feautres.Staff.Query
{
    public record StaffListItemDto(
    string Id, string Email, string FullName, bool IsActive,
    string? JobTitle, string? Department, string? ReportsToId, IReadOnlyList<string> Roles);

    public record GetStaffQuery(string CallerId, bool AllTeams, string? Role, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<StaffListItemDto>>>;

}
