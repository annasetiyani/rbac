using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Entities;

public class Role
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string RoleName { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = null!;
    public ICollection<RolePermission> RolePermissions { get; set; }=null!;
}
