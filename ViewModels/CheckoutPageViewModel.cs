using System.ComponentModel.DataAnnotations;

namespace Product_Details.ViewModels
{
    public class CheckoutPageViewModel
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ImagePath { get; set; }

        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue,
            ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid mobile number.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^[1-9][0-9]{5}$",
            ErrorMessage = "Enter a valid pincode.")]
        public string? Pincode { get; set; }

        public bool ChangeAddress { get; set; }

        [RegularExpression(@"^[A-Za-z ]*$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? NewName { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid mobile number.")]
        public string? NewPhone { get; set; }

        public string? NewAddress { get; set; }

        [RegularExpression(@"^[A-Za-z ]*$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? NewCity { get; set; }

        [RegularExpression(@"^[1-9][0-9]{5}$",
            ErrorMessage = "Enter a valid pincode.")]
        public string? NewPincode { get; set; }

        public int Stock { get; set; }
    }
}