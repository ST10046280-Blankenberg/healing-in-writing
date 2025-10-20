using Microsoft.AspNetCore.Mvc;
using HealingInWriting.Models.Auth;

namespace HealingInWriting.Controllers
{
    // TODO: Inject an authentication service and defer all auth workflows to it.
    public class AuthController : Controller
    {
        // GET: /Auth/Auth - Unified authentication page
        [HttpGet]
        public IActionResult Auth()
        {
            return View();
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Redirect to unified Auth page with register state
            return RedirectToAction("Auth", new { mode = "register" });
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Auth", model);
            }

            // TODO: Implement registration logic via authentication service
            // For now, just redirect to a success page or login
            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login", "Auth");
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Redirect to unified Auth page with login state
            return RedirectToAction("Auth", new { mode = "login" });
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVm model)
        {
            if (!ModelState.IsValid)
            {
                return View("Auth", model);
            }

            // TODO: Implement login logic via authentication service
            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToAction("Index", "Home");
        }
    }
}
