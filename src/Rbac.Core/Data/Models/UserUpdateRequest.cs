using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Models;

public class UserUpdateRequest
{
    [Required]    
    public required Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [RegularExpression("^[a-zA-Z0-9._|\\\\%#~`=?&/$^*!}{+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}"
        , ErrorMessage = "Password must meet requirements")]
    public required string Email { get; set; }
    public IList<Guid> RoleIds { get; set; } = null!;

}
