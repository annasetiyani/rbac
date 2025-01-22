using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;

namespace Rbac.Core.Services.Interfaces;

public interface IRoleProvider
{
    Task<Role?> GetRoleByIdAsync(Guid roleId);
    Task<IList<RolePermissionItem>?> GetRolePermissionItemsAsync(IEnumerable<Guid> ids);
    Task<IList<RolePermissionItem>?> GetRolesAsync();
}
