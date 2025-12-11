using System.ComponentModel.DataAnnotations;

namespace PRM.Dtos
{
    public class RegisterDtos
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string firstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string lastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        [DataType(DataType.Password)]
        public string password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Password and Confirm Password do not match")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
