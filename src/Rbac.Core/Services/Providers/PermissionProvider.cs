using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rbac.Core.Data;
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;
using Rbac.Core.Services.Interfaces;
using System.Linq;

namespace Rbac.Core.Services.Providers;

public class PermissionProvider : IPermissionProvider
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public PermissionProvider(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<IList<PermissionItem>?> GetPermissionItemsAsync()
    {
        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(x => x.PermissionName)
            .ToListAsync();
        if(permissions == null || !permissions.Any())
            return null!;

        return permissions?.Select(ToPermissionItem).ToList();
    }

    public async Task<IList<PermissionItem>?> GetPermissionsAsync(IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return null;

        var permissions  = await dbContext.Permissions
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        return permissions?.Select(ToPermissionItem)?.ToList();
    }
    public async Task<PermissionItem> GetPermissionItemAsync(Guid id)
    {
        var permission = await dbContext.Permissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);
        return permission != null ? mapper.Map<PermissionItem>(permission) : null!;
    }
    private PermissionItem ToPermissionItem(Permission permission)
    {
        return mapper.Map<PermissionItem>(permission);
    }

    
}
