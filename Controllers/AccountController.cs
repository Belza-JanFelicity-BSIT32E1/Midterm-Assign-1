using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;


namespace Midterm_Assign_1.Controllers

    {
        public class AccountController : Controller
        {
            private static readonly Dictionary<string, string> RegisteredUsers = new Dictionary<string, string>();
            private static readonly Dictionary<string, int> LoginAttempts = new Dictionary<string, int>();

            // Initialize some demo users (username:password)
            static AccountController()
            {
                RegisteredUsers.Add("user1", HashPassword("password1"));
                RegisteredUsers.Add("user2", HashPassword("password2"));
            }

            public ActionResult Login()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Login(string username, string password)
            {
                if (ModelState.IsValid)
                {
                    if (RegisteredUsers.ContainsKey(username))
                    {
                        if (VerifyPassword(password, RegisteredUsers[username]))
                        {
                            // Clear login attempts
                            LoginAttempts.Remove(username);

                            // Redirect to dashboard or home page
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Increase login attempt count
                            int attempts = LoginAttempts.ContainsKey(username) ? LoginAttempts[username] : 0;
                            LoginAttempts[username] = attempts + 1;

                            // Check for exceeding login attempt limit
                            if (attempts >= 2)
                            {
                                // Reset login attempt count
                                LoginAttempts.Remove(username);
                                ModelState.AddModelError("", "You have exceeded the maximum number of login attempts. Please consider re-registration.");
                            }
                            else
                            {
                                ModelState.AddModelError("", $"{3 - attempts} attempts remaining.");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
          
            return View();


            }

            // Helper method to hash password
            private static string HashPassword(string password)
            {
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512);
                byte[] hash = pbkdf2.GetBytes(20);

                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                return Convert.ToBase64String(hashBytes);
            }

            // Helper method to verify password
            private static bool VerifyPassword(string enteredPassword, string storedPassword)
            {
                byte[] hashBytes = Convert.FromBase64String(storedPassword);

                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA512);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }