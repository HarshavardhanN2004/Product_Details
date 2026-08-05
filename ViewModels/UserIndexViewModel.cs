using Product_Details.Models;

namespace Product_Details.ViewModels
{
    public class UserIndexViewModel
    {

        public List<Product> Products { get; set; } = new();

        public CheckoutViewModel Checkout { get; set; } = new();
    }
}
