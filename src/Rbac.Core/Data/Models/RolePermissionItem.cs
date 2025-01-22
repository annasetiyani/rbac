namespace Rbac.Core.Data.Models;

public class RolePermissionItem :RoleItem
{
    public IList<PermissionItem>? Permissions { get; set; } = null!;
}
