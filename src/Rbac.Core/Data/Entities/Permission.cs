using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Entities;

public class Permission
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string PermissionName { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }= null!;
}
