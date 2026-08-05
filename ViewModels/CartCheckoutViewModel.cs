using Product_Details.Models;
using System.ComponentModel.DataAnnotations;
namespace Product_Details.ViewModels
{
    public class CartCheckoutViewModel
    {
        public List<CartViewModel> CartItems { get; set; } = new();

        public decimal GrandTotal { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Pincode { get; set; }

        public bool ChangeAddress { get; set; }

        public string? NewName { get; set; }

        public string? NewPhone { get; set; }

        public string? NewAddress { get; set; }

        public string? NewCity { get; set; }

        public string? NewPincode { get; set; }
    }
}