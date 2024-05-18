using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using YourNamespace.Models;

namespace YourNamespace.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Remote("IsUsernameAvailable", "Account", HttpMethod = "POST", ErrorMessage = "Username already exists.")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}

namespace YourNamespace.Controllers
{
    public class AccountController : Controller
    {
        private static readonly List<string> RegisteredUsernames = new List<string>();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!RegisteredUsernames.Contains(model.Username))
                {
                    // Store username temporarily (demonstration purposes only)
                    RegisteredUsernames.Add(model.Username);

                    // Redirect to login or show confirmation message
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult IsUsernameAvailable(string username)
        {
            return Json(!RegisteredUsernames.Contains(username));
        }
    }
}