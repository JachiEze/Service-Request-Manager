using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceRequestForm.Data;
using ServiceRequestForm.Models;
using ServiceRequestForm.Services;
using System.Linq;
using System;

namespace ServiceRequestForm.Controllers
{
    public class AccountController : Controller


    {
        private readonly ApplicationDbContext _context;

        private readonly EmailService _emailService;

public AccountController(ApplicationDbContext context, EmailService emailService)
{
    _context = context;
    _emailService = emailService;
}


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.UserAccounts
                .FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var currentSessionId = HttpContext.Session.Id;
            var now = DateTime.UtcNow;


            if (!string.IsNullOrEmpty(user.ActiveSessionId) && user.ActiveSessionId != currentSessionId)
            {
                if (user.LastActivity.HasValue && user.LastActivity.Value > now.AddMinutes(-15))
                {
                    
                    ModelState.AddModelError("", "This account is already logged in from another device.");
                    return View(model);
                }
                else
                {
                
                    user.ActiveSessionId = null;
                    user.LastActivity = null;
                    _context.SaveChanges();
                }
            }

                user.ActiveSessionId = currentSessionId;
    user.LastActivity = now;
    _context.SaveChanges();


            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            _emailService.SendLoginNotification(user.Username);

            return user.Role switch
            {
                "Requester" => RedirectToAction("Index", "User1"),
                "Validator" => RedirectToAction("Index", "User2"),
                "Approver" => RedirectToAction("Index", "User3"),
                _ => RedirectToAction("Index", "Home")
            };
        }



        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Username.Equals("validator", StringComparison.OrdinalIgnoreCase) ||
       model.Username.Equals("approver", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "You cannot register as a validator or approver.");
                return View(model);
            }


            if (_context.UserAccounts.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("", "Username already taken. Please choose another one.");
                return View(model);
            }


            var newUser = new UserAccount
            {
                Username = model.Username,
                Password = model.Password,
                Role = "Requester"
            };

            _context.UserAccounts.Add(newUser);
            _context.SaveChanges();


            HttpContext.Session.SetString("Username", newUser.Username);
            HttpContext.Session.SetString("Role", newUser.Role);

            return RedirectToAction("Index", "User1");
        }


        public IActionResult Logout()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username != null)
            {
                var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    user.ActiveSessionId = null;
                    user.LastActivity = null;
                    _context.SaveChanges();
                }
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}






