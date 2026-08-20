using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Product_Details.Data;
using Product_Details.Models;
using Product_Details.ViewModels;

namespace Product_Details.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string? search, int page = 1)
        {
            string role = HttpContext.Session.GetString("Role");

            if (role == "User")
            {
                return RedirectToAction("UserIndex");
            }

            int pageSize = 5;

            var products = _context.Products.AsQueryable();

           
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(search));
            }

            
            int totalProducts = products.Count();

           
            int totalPages = (int)Math.Ceiling(
                (double)totalProducts / pageSize);

          
            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(paginatedProducts);
        }

        public IActionResult UserIndex(string search,string priceRange,int page = 1)
        {
            int pageSize = 9;

            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(search) ||
                    p.Type.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                var prices = priceRange.Split('-');

                if (prices.Length == 2)
                {
                    decimal minPrice;
                    decimal maxPrice;

                    if (decimal.TryParse(prices[0], out minPrice) &&
                        decimal.TryParse(prices[1], out maxPrice))
                    {
                        products = products.Where(p =>
                            p.Price >= minPrice &&
                            p.Price <= maxPrice);
                    }
                }
            }

            int totalProducts = products.Count();

            int totalPages = (int)Math.Ceiling(
                (double)totalProducts / pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var paginatedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users
                               .FirstOrDefault(x => x.UserName == username);

            var vm = new UserIndexViewModel
            {
                Products = paginatedProducts,
                Checkout = new CheckoutViewModel()
            };

            if (user != null)
            {
                vm.Checkout.Name = user.UserName;
                vm.Checkout.Phone = user.Phone;
                vm.Checkout.Address = user.Address;
                vm.Checkout.City = user.City;
                vm.Checkout.Pincode = user.Pincode;
            }

            ViewBag.Search = search;
            ViewBag.PriceRange = priceRange;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewBag.CartCount = GetCartCount();

            return View(vm);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
              
                if (imageFile != null && imageFile.Length > 0)
                {
                  
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    product.ImagePath = "/images/" + fileName;
                }

                _context.Products.Add(product);
                _context.SaveChanges();
                TempData["Success"] = "Product added successfully!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    product.ImagePath = "/images/" + fileName;
                }

                _context.Products.Update(product);
                _context.SaveChanges();

                TempData["Success"] = "Product updated successfully!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult DeleteConfirmed(int ProductId)
        {
            var product = _context.Products.Find(ProductId);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = _context.Carts
                .FirstOrDefault(x => x.ProductId == productId &&
                                     x.UserId == user.UserId);

            if (cart == null)
            {
                cart = new Cart()
                {
                    ProductId = productId,
                    UserId = user.UserId,
                    Quantity = 1
                };

                _context.Carts.Add(cart);
            }
            else
            {
                cart.Quantity++;
            }

            _context.SaveChanges();

            TempData["Success"] = "Product added to cart.";

            return RedirectToAction("UserIndex");
        }
        public IActionResult Cart()
        {
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Carts
                            .Where(c => c.UserId == user.UserId)
                            .Join(_context.Products,
                                  c => c.ProductId,
                                  p => p.ProductId,
                                  (c, p) => new CartViewModel
                                  {
                                      CartId = c.CartId,
                                      ProductId = p.ProductId,
                                      ProductName = p.ProductName,
                                      ImagePath = p.ImagePath,
                                      Price = p.Price,
                                      Quantity = c.Quantity,
                                      Total = p.Price * c.Quantity
                                  })
                            .ToList();

            ViewBag.CartCount = GetCartCount();
            return View(cartItems);
        }

        public IActionResult IncreaseQuantity(int id)
        {
            var cart = _context.Carts.Find(id);

            if (cart != null)
            {
                var product = _context.Products.Find(cart.ProductId);

                if (product != null && cart.Quantity < product.Quantity)
                {
                    cart.Quantity++;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Cart");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var cart = _context.Carts.Find(id);

            if (cart != null)
            {
                if (cart.Quantity > 1)
                {
                    cart.Quantity--;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Cart");
        }

        public IActionResult RemoveCart(int id)
        {
            var cart = _context.Carts.Find(id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();

                TempData["Success"] = "Product removed from cart.";
            }

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult BuyNow(int id)
        {
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Products.FirstOrDefault(x => x.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            CheckoutPageViewModel vm = new CheckoutPageViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ImagePath = product.ImagePath,
                Price = product.Price,
                Quantity = 1,
                Stock = product.Quantity,
                TotalAmount = product.Price,

                Name = user.UserName,
                Phone = user.Phone,
                Address = user.Address,
                City = user.City,
                Pincode = user.Pincode,

                NewName = user.UserName,
                NewPhone = user.Phone,
                NewAddress = user.Address,
                NewCity = user.City,
                NewPincode = user.Pincode
            };

            ViewBag.CartCount = GetCartCount();
            return View(vm);
        }

        [HttpGet]
        public IActionResult BuyAll()
        {
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Carts
                .Where(c => c.UserId == user.UserId)
                .Select(c => new CartViewModel
                {
                    CartId = c.CartId,
                    ProductId = c.ProductId,
                    ProductName = c.Product.ProductName,
                    ImagePath = c.Product.ImagePath,
                    Price = c.Product.Price,
                    Quantity = c.Quantity,
                    Total = c.Product.Price * c.Quantity,
                    Stock = c.Product.Quantity
                })
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            CartCheckoutViewModel vm = new CartCheckoutViewModel
            {
                CartItems = cartItems,

                GrandTotal = cartItems.Sum(x => x.Total),

                Name = user.UserName,
                Phone = user.Phone,
                Address = user.Address,
                City = user.City,
                Pincode = user.Pincode,

                NewName = user.UserName,
                NewPhone = user.Phone,
                NewAddress = user.Address,
                NewCity = user.City,
                NewPincode = user.Pincode
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult PlaceAllOrders(CartCheckoutViewModel model)
        {
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Carts
                .Where(x => x.UserId == user.UserId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Cart is empty.";
                return RedirectToAction("Cart");
            }

            if (model.ChangeAddress)
            {
                if (string.IsNullOrWhiteSpace(model.NewName))
                    ModelState.AddModelError("NewName", "Name is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewName, @"^[A-Za-z ]+$"))
                    ModelState.AddModelError("NewName", "Only alphabets are allowed.");

                if (string.IsNullOrWhiteSpace(model.NewPhone))
                    ModelState.AddModelError("NewPhone", "Phone Number is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewPhone, @"^[6-9]\d{9}$"))
                    ModelState.AddModelError("NewPhone", "Enter a valid phone number.");

                if (string.IsNullOrWhiteSpace(model.NewAddress))
                    ModelState.AddModelError("NewAddress", "Address is required.");

                if (string.IsNullOrWhiteSpace(model.NewCity))
                    ModelState.AddModelError("NewCity", "City is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewCity, @"^[A-Za-z ]+$"))
                    ModelState.AddModelError("NewCity", "Only alphabets are allowed.");

                if (string.IsNullOrWhiteSpace(model.NewPincode))
                    ModelState.AddModelError("NewPincode", "Pincode is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewPincode, @"^[1-9][0-9]{5}$"))
                    ModelState.AddModelError("NewPincode", "Enter valid pincode.");
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key} : {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var item in model.CartItems)
                {
                    var product = _context.Products.Find(item.ProductId);

                    if (product != null)
                    {
                        item.ProductName = product.ProductName;
                        item.ImagePath = product.ImagePath;
                        item.Price = product.Price;
                        item.Total = product.Price * item.Quantity;
                        item.Stock = product.Quantity;
                    }
                }

                model.GrandTotal = model.CartItems.Sum(x => x.Total);
                return View("BuyAll", model);
            }

            string name = model.Name;
            string phone = model.Phone;
            string address = model.Address;
            string city = model.City;
            string pincode = model.Pincode;

            if (model.ChangeAddress)
            {
                name = model.NewName;
                phone = model.NewPhone;
                address = model.NewAddress;
                city = model.NewCity;
                pincode = model.NewPincode;
            }

            foreach (var cart in cartItems)
            {
                var product = _context.Products.Find(cart.ProductId);

                if (product == null)
                    continue;

                if (cart.Quantity > product.Quantity)
                {
                    TempData["Error"] = $"{product.ProductName} is out of stock.";
                    return RedirectToAction("Cart");
                }

                Order order = new Order()
                {
                    ProductId = product.ProductId,
                    CustomerId = user.UserId,
                    QuantityOrdered = cart.Quantity,
                    TotalAmount = cart.Quantity * product.Price,
                    OrderDate = DateTime.Now,

                    Name = name,
                    Phone = phone,
                    Address = address,
                    City = city,
                    Pincode = pincode
                };

                _context.Orders.Add(order);

                product.Quantity -= cart.Quantity;
            }

            _context.Carts.RemoveRange(cartItems);

            _context.SaveChanges();

            TempData["Success"] = "Order placed successfully.";

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        public IActionResult PlaceOrder(CheckoutPageViewModel model)
        {
            foreach (var item in ModelState)
            {
                foreach (var error in item.Value.Errors)
                {
                    Console.WriteLine($"{item.Key} : {error.ErrorMessage}");
                }
            }
            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Products.Find(model.ProductId);

            if (!ModelState.IsValid)
            {
                model.ProductName = product.ProductName;
                model.ImagePath = product.ImagePath;
                model.Price = product.Price;
                model.Stock = product.Quantity;
                model.TotalAmount = model.Price * model.Quantity;
                return View("BuyNow", model);
            }

          

            if (product == null)
            {
                return NotFound();
            }

            if (model.Quantity > product.Quantity)
            {
                ModelState.AddModelError("Quantity",
                    "Requested quantity is not available.");

                model.ProductName = product.ProductName;
                model.ImagePath = product.ImagePath;
                model.Price = product.Price;
                model.Stock = product.Quantity;
                model.TotalAmount = model.Price * model.Quantity;

                return View("BuyNow", model);
            }

            string name = model.Name;
            string phone = model.Phone;
            string address = model.Address;
            string city = model.City;
            string pincode = model.Pincode;

            if (model.ChangeAddress)
            {
                if (string.IsNullOrWhiteSpace(model.NewName))
                    ModelState.AddModelError("NewName", "Name is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewName, @"^[A-Za-z ]+$"))
                    ModelState.AddModelError("NewName", "Only alphabets are allowed.");

                if (string.IsNullOrWhiteSpace(model.NewPhone))
                    ModelState.AddModelError("NewPhone", "Phone Number is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewPhone, @"^[6-9]\d{9}$"))
                    ModelState.AddModelError("NewPhone", "Enter a valid 10 digit phone number.");

                if (string.IsNullOrWhiteSpace(model.NewAddress))
                    ModelState.AddModelError("NewAddress", "Address is required.");

                if (string.IsNullOrWhiteSpace(model.NewCity))
                    ModelState.AddModelError("NewCity", "City is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewCity, @"^[A-Za-z ]+$"))
                    ModelState.AddModelError("NewCity", "Only alphabets are allowed.");

                if (string.IsNullOrWhiteSpace(model.NewPincode))
                    ModelState.AddModelError("NewPincode", "Pincode is required.");

                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.NewPincode, @"^[1-9][0-9]{5}$"))
                    ModelState.AddModelError("NewPincode", "Enter a valid pincode.");

                if (!ModelState.IsValid)
                {
                    model.ProductName = product.ProductName;
                    model.ImagePath = product.ImagePath;
                    model.Price = product.Price;
                    model.Stock = product.Quantity;
                    model.TotalAmount = model.Price * model.Quantity;

                    return View("BuyNow", model);
                }

                name = model.NewName;
                phone = model.NewPhone;
                address = model.NewAddress;
                city = model.NewCity;
                pincode = model.NewPincode;
            }

            Order order = new Order()
            {
                ProductId = product.ProductId,
                CustomerId = user.UserId,

                QuantityOrdered = model.Quantity,

                TotalAmount = model.Quantity * product.Price,

                OrderDate = DateTime.Now,

                Name = name,
                Phone = phone,
                Address = address,
                City = city,
                Pincode = pincode
            };

            _context.Orders.Add(order);

            product.Quantity -= model.Quantity;

            _context.SaveChanges();

            TempData["Success"] = "Order placed successfully.";

            return RedirectToAction("UserIndex");
        }
        public int GetCartCount()
        {
            string username = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(username))
                return 0;

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
                return 0;

            return _context.Carts
                           .Where(x => x.UserId == user.UserId)
                           .Sum(x => x.Quantity);
        }
    }
}