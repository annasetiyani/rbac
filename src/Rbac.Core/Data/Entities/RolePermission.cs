using System.ComponentModel.DataAnnotations.Schema;

namespace Rbac.Core.Data.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role Role { get; set; } = null!;
    public Guid PermissionId { get; set; }
    [ForeignKey("PermissionId")]
    public Permission Permission { get; set; }= null!;
}
