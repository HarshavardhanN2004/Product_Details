using System.ComponentModel.DataAnnotations;

namespace Product_Details.Models
{
    public class Cart
    {

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
