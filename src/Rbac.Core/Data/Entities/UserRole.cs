using System.ComponentModel.DataAnnotations.Schema;

namespace Rbac.Core.Data.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    public Guid RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role Role { get; set; } = null!;
}
