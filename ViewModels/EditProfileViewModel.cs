using System.ComponentModel.DataAnnotations;

namespace Product_Details.ViewModels
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Username should contain only alphabets.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Phone number must start with 6-9 and contain exactly 10 digits.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^[1-9][0-9]{5}$",
            ErrorMessage = "Enter a valid pincode.")]
        public string Pincode { get; set; }
    }
}