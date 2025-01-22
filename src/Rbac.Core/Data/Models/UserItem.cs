namespace Rbac.Core.Data.Models;

public class UserItem
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } 
    public IList<RolePermissionItem>? RolePermissions { get; set; } = null!;
}
