using Microsoft.AspNetCore.Mvc;
using HealingInWriting.Models.Auth;

namespace HealingInWriting.Controllers
{
    // TODO: Inject an authentication service and defer all auth workflows to it.
    public class AuthController : Controller
    {
        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
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
            // TODO: Implement login view
            return View();
        }
    }
}
