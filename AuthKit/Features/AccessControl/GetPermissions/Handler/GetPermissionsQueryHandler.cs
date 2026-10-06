using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetPermissions.Handler
{
    public class GetPermissionsQueryHandler<TContext> : IRequestHandler<GetPermissionsQuery, Result<List<PermissionDto>>>
     where TContext : DbContext
    {
        private readonly TContext _dbContext;
        public GetPermissionsQueryHandler(TContext dbContext) => _dbContext = dbContext;

        public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _dbContext.Set<Permission>()
                .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
                .ToListAsync(cancellationToken);

            return Result<List<PermissionDto>>.Success(permissions);
        }
    }
}
