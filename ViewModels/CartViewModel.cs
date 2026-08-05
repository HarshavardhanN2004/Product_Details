namespace Product_Details.ViewModels
{
    public class CartViewModel
    {
        public int CartId { get; set; }

        public int ProductId { get; set; }

        public string?  ProductName { get; set; }

        public string? ImagePath { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal Total { get; set; }

        public int Stock { get; set; }
    }
}
