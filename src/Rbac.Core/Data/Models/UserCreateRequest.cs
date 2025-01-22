using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rbac.Core.Data.Models;

public class UserCreateRequest
{
    [Required]
    [MaxLength(200)]
    public required string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[A-Z])(?=.*\\d)(?=.*[a-zA-Z])(?=\\S+$).{8,20}$"
        , ErrorMessage = "Password must meet requirements")]
    [MaxLength(20)]
    [MinLength(8)]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [MaxLength(20)]
    [MinLength(8)]
    [NotMapped]
    public required string ConfirmPassword { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [RegularExpression("^[a-zA-Z0-9._|\\\\%#~`=?&/$^*!}{+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}"
        , ErrorMessage = "Password must meet requirements")]
    public required string Email { get; set; }

    [Required]
    public required Guid RoleId { get; set; }
}
