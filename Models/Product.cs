using System.ComponentModel.DataAnnotations;
namespace Product_Details.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Product Name should contain only alphabets.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [RegularExpression(@"^[A-Za-z ]+$",
            ErrorMessage = "Type should contain only alphabets.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public string? ImagePath { get; set; }
    }
}