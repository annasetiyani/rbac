using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Models;

public class UserForgotPasswordRequest
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
    public required string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    [MaxLength(20)]
    [MinLength(8)]
    [NotMapped]
    public required string ConfirmPassword { get; set; }
}