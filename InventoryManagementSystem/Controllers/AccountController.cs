using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Models.VM;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        protected readonly IUserService _userService;
        
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            

            UserModel user = _userService.GetUserWithRole(model.Email, model.Password);
            if(user == null)
            {
                ViewData["Login Error "] = "Please Enter correct email and password";
                return View("Index");
            }
            if(model.Password == user.Password)
            {
                user.ConfirmPassword = user.Password;
                model.Username = user.Username;
            }
            else
            {
                return View("Index");
            }
            if (user != null)
            {
                IdentityUtils.AddingClaimIdentity(model, user.Roles ?? "employee", HttpContext);
                return Redirect("/");
            }
            else
                ModelState.AddModelError("Password", "Invalid Username or Password");

            return View("Index");
        }
        [HttpGet]

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        public IActionResult SignUp()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Employee", Text = "Employee" },
                new SelectListItem { Value = "Admin", Text = "Admin" }
            };
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserModel
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Roles = "Employee",
                    ConfirmPassword = model.ConfirmPassword,
                };


                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Employee", Text = "Employee" },
                new SelectListItem { Value = "Admin", Text = "Admin" }
            };

            return View(model);
        }
    }
}
