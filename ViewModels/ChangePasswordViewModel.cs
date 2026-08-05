using System.ComponentModel.DataAnnotations;

namespace Product_Details.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Existing Password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(15,MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 15 characters.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&^#()_+\-=\[\]{};':""\\|,.<>\/]).{6,15}$",
            ErrorMessage = "Password must contain one uppercase letter, one lowercase letter, one number and one special character.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("NewPassword",ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}