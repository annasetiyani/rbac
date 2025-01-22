using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string Username { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; } 
    [Required]
    public required string PasswordHash { get; set; }
    [Required]
    public required bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = null!;
}
