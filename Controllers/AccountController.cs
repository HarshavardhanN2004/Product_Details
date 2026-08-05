using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Details.Data;
using Product_Details.Models;
using Product_Details.ViewModels;
namespace Product_Details.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(x =>
                    x.UserName == login.UserName &&
                    x.Password == login.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetInt32("UserId", user.UserId);

                    TempData["Success"] = "Login Successful!";

                    return RedirectToAction("Index", "Product");
                }

                TempData["Error"] = "Wrong Username or Password";
            }

            return View(login);
        }
        public IActionResult AdminDashboard()
        {
            return Content("Welcome Admin");
        }

        public IActionResult UserDashboard()
        {
            return Content("Welcome User");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["Success"] = "Logout Successful!";

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                bool userExists = _context.Users.Any(x => x.UserName == user.UserName);

                if (userExists)
                {
                    ModelState.AddModelError("UserName", "Username already exists.");
                    return View(user);
                }

                bool emailExists = _context.Users.Any(x => x.Email == user.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(user);
                }

                user.Role = "User";

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] = "Registration Successful.";

                return RedirectToAction("Login");
            }

            return View(user);
        }

        public IActionResult Profile()
        {
            string username = HttpContext.Session.GetString("UserName");

            if (username == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users
                               .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult EditProfile(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            EditProfileViewModel model = new EditProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                City = user.City,
                Pincode = user.Pincode
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(model.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Phone = model.Phone;
                user.Email = model.Email;
                user.Address = model.Address;
                user.City = model.City;
                user.Pincode = model.Pincode;

                _context.SaveChanges();

                HttpContext.Session.SetString("UserName", user.UserName);

                TempData["Success"] = "Profile updated successfully.";

                return RedirectToAction("Profile");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string username = HttpContext.Session.GetString("UserName");

            var user = _context.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword",
                    "Old password is incorrect.");

                return View(model);
            }

            if (model.OldPassword == model.NewPassword)
            {
                ModelState.AddModelError("NewPassword",
                    "New password must be different.");

                return View(model);
            }

            user.Password = model.NewPassword;
            user.ConfirmPassword = model.NewPassword;

            _context.SaveChanges();

            HttpContext.Session.Clear();

            TempData["Success"] =
                "Password changed successfully. Please login again.";

            return RedirectToAction("Login");
        }
    }
}
