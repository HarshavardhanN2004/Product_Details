using System.ComponentModel.DataAnnotations;
namespace Product_Details.ViewModels
{
    public class CheckoutViewModel
    {
        [Key]
        public int ProductId { get; set; }

        public string OrderType { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression(@"^[1-9][0-9]{5}$",
            ErrorMessage = "Enter a valid Pincode.")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int QuantityOrdered { get; set; }

        public bool IsAddressChanged { get; set; }

        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? NewName { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10 digit mobile number.")]
        public string? NewPhone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? NewAddress { get; set; }

        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Only alphabets are allowed.")]
        public string? NewCity { get; set; }

        [RegularExpression(@"^[1-9][0-9]{5}$",
            ErrorMessage = "Enter a valid 6 digit pincode.")]
        public string? NewPincode { get; set; }
    }
}