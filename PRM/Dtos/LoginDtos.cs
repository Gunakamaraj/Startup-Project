using System.ComponentModel.DataAnnotations;

namespace PRM.Dtos
{
    public class LoginDtos
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;
    }
}
