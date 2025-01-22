using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Models;

public class PermissionItem
{
    public Guid Id { get; set; }
    public string PermissionName { get; set; } = null!;
}
