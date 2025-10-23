using Microsoft.AspNetCore.Mvc;
using HealingInWriting.Models.Auth;
using HealingInWriting.Interfaces.Services;

namespace HealingInWriting.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication operations including
    /// registration, login, email verification, and logout.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// GET: /Auth/Auth - Displays the unified authentication page.
        /// Supports both login and register modes via query parameter.
        /// </summary>
        /// <param name="mode">Optional mode parameter: "login" or "register"</param>
        [HttpGet]
        public IActionResult Auth(string? mode)
        {
            // Clear any previous messages when displaying fresh form
            return View();
        }

        /// <summary>
        /// GET: /Auth/Register - Redirects to unified auth page in register mode.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction("Auth", new { mode = "register" });
        }

        /// <summary>
        /// POST: /Auth/Register - Handles user registration with email verification.
        /// </summary>
        /// <param name="model">Registration data including name, email, and password</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return to register form with validation errors
                ViewData["Mode"] = "register";
                return View("Auth", model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Auth", new { mode = "login" });
            }

            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["Mode"] = "register";
            return View("Auth", model);
        }

        /// <summary>
        /// GET: /Auth/Login - Redirects to unified auth page in login mode.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction("Auth", new { mode = "login" });
        }

        /// <summary>
        /// POST: /Auth/Login - Handles user login with email verification check.
        /// </summary>
        /// <param name="model">Login credentials including email, password, and remember me</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return to login form with validation errors
                ViewData["Mode"] = "login";
                return View("Auth", model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success)
            {
                // Successful login - redirect to landing page (Home/Index)
                TempData["SuccessMessage"] = "Welcome back!";
                return RedirectToAction("Index", "Home");
            }

            // Login failed - could be invalid credentials or unverified email
            ModelState.AddModelError(string.Empty, result.Message);
            ViewData["Mode"] = "login";
            return View("Auth", model);
        }

        /// <summary>
        /// GET: /Auth/VerifyEmail - Verifies user's email address using token from email link.
        /// </summary>
        /// <param name="userId">The user's unique identifier</param>
        /// <param name="token">The email verification token</param>
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid verification link.";
                return RedirectToAction("Auth", new { mode = "login" });
            }

            var result = await _authService.VerifyEmailAsync(userId, token);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("EmailVerificationSuccess");
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Auth", new { mode = "login" });
        }

        /// <summary>
        /// GET: /Auth/EmailVerificationPending - Displays message to check email for verification.
        /// </summary>
        [HttpGet]
        public IActionResult EmailVerificationPending()
        {
            return View();
        }

        /// <summary>
        /// GET: /Auth/EmailVerificationSuccess - Displays success message after email verification.
        /// </summary>
        [HttpGet]
        public IActionResult EmailVerificationSuccess()
        {
            return View();
        }

        /// <summary>
        /// POST: /Auth/Logout - Logs out the current user and redirects to login page.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Auth", new { mode = "login" });
        }

        // TODO [Future Enhancement]: Add ForgotPassword and ResetPassword actions when required
        // [HttpGet]
        // public IActionResult ForgotPassword() => View();
        //
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) { }
        //
        // [HttpGet]
        // public IActionResult ResetPassword(string userId, string token) => View();
        //
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) { }
    }
}
