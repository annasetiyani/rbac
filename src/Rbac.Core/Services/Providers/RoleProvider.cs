using Microsoft.EntityFrameworkCore;
using Rbac.Core.Data;
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;
using Rbac.Core.Services.Interfaces;

namespace Rbac.Core.Services.Providers;

public class RoleProvider(AppDbContext dbContext, IPermissionProvider permissionProvider) 
    : IRoleProvider
{
    private readonly AppDbContext dbContext = dbContext;
    private readonly IPermissionProvider permissionProvider = permissionProvider;

    public async Task<Role?> GetRoleByIdAsync(Guid roleId)
    {
        return await dbContext.Roles
            .AsNoTracking()
            .Include(nameof(Role.RolePermissions))
            .FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public async Task<IList<RolePermissionItem>?> GetRolePermissionItemsAsync(IEnumerable<Guid> ids)
    {
        if(ids == null || !ids.Any())
            return null!;
        var rolePermissionItems = new List<RolePermissionItem>();
        
        var roles = await dbContext.Roles
            .AsNoTracking()
            .Include(nameof(Role.RolePermissions))
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        if (roles == null || roles.Count < 0)
            return null!;
        var permissionIds = roles
            .SelectMany(x => x.RolePermissions.Select(x => x.PermissionId))
            .Distinct().ToList();

        var permissionItems = await permissionProvider.GetPermissionsAsync(permissionIds);
        foreach (var role in roles)
        {
            var rolePermissionItem = ToRolePermissionItemAsync(role, permissionItems);
            rolePermissionItems.Add(rolePermissionItem);
        }

        return rolePermissionItems;

    }

    private static RolePermissionItem ToRolePermissionItemAsync(Role role, IList<PermissionItem>? permissionItems)
    {
        var result = new RolePermissionItem
        {
            Id = role.Id,
            RoleName = role.RoleName
        };
        if (role.RolePermissions == null || !role.RolePermissions.Any())
        {
            return result;
        }
        result.Permissions = new List<PermissionItem>();
        foreach (var item in role.RolePermissions)
        {
            var rp = permissionItems?.FirstOrDefault(x => x.Id == item.PermissionId);
            if (rp != null)
                result.Permissions.Add(rp);
        }
        return result;
    }

    public async Task<IList<RolePermissionItem>?> GetRolesAsync()
    {
        var roles = await dbContext.Roles
            .AsNoTracking()
            .Include(nameof(Role.RolePermissions))
            .OrderBy(x=>x.RoleName)
            .ToListAsync();
        if (roles == null || roles.Count < 0)
            return null!;
        var permissionItems = await permissionProvider.GetPermissionItemsAsync();

        var rolePermissionItems = new List<RolePermissionItem>();
        foreach (var role in roles)
        {
            var rolePermissionItem = new RolePermissionItem
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
            if (role.RolePermissions == null || !role.RolePermissions.Any())
            {
                rolePermissionItems.Add(rolePermissionItem);
                continue;
            }

            rolePermissionItem.Permissions = new List<PermissionItem>();
            foreach (var item in role.RolePermissions)
            {
                var rp = permissionItems?.FirstOrDefault(x => x.Id == item.PermissionId);
                if (rp != null)
                    rolePermissionItem.Permissions.Add(rp);
            }
            rolePermissionItems.Add(rolePermissionItem);
        }
        return rolePermissionItems;
    }

    
}
