using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Models;

public class RoleItem
{
    public Guid Id { get; set; }
    
    public string RoleName { get; set; } = null!;
}
