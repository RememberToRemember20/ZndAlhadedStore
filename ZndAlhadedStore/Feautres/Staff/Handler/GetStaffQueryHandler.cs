using AuthKit.Common;
using AuthKit.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.Staff.Query;

namespace ZndAlhadedStore.Feautres.Staff.Handler
{
    public class GetStaffQueryHandler : IRequestHandler<GetStaffQuery, Result<PagedResult<StaffListItemDto>>>
    {
        private readonly AppDbContext _db;
        public GetStaffQueryHandler(AppDbContext db) => _db = db;

        public async Task<Result<PagedResult<StaffListItemDto>>> Handle(GetStaffQuery r, CancellationToken ct)
        {
            var query = _db.StaffProfiles.AsNoTracking().AsQueryable();

            if (!r.AllTeams)
                query = query.Where(s => s.ReportsToId == r.CallerId);

            if (!string.IsNullOrWhiteSpace(r.Role))
            {
                var normalized = r.Role.Trim().ToUpperInvariant();
                query = query.Where(s => _db.UserRoles.Any(ur => ur.UserId == s.UserId &&
                    _db.Roles.Any(role => role.Id == ur.RoleId && role.NormalizedName == normalized)));
            }

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderBy(s => s.User.FullName).ThenBy(s => s.UserId)
                .Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
                .Select(s => new StaffListItemDto(
                    s.UserId, s.User.Email!, s.User.FullName, s.User.IsActive,
                    s.JobTitle, s.Department, s.ReportsToId,
                    _db.UserRoles.Where(ur => ur.UserId == s.UserId)
                        .Join(_db.Roles, ur => ur.RoleId, role => role.Id, (ur, role) => role.Name!)
                        .ToList()))
                .ToListAsync(ct);

            return Result<PagedResult<StaffListItemDto>>.Success(new PagedResult<StaffListItemDto>(items, r.Page, r.PageSize, total));
        }
    }
}
