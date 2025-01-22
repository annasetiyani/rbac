using System.ComponentModel.DataAnnotations;

namespace Rbac.Core.Data.Models
{
    public class UserLoginRequest
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
    }
}
