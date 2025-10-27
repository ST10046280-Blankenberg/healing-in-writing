using HealingInWriting.Models.Auth;

namespace HealingInWriting.Interfaces.Services;

/// <summary>
/// Service interface for handling authentication operations including registration,
/// login, and email verification.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with email verification required.
    /// </summary>
    /// <param name="model">The registration data including email, password, and name.</param>
    /// <returns>
    /// A task containing a tuple with:
    /// - bool: Success status
    /// - string: Error message if failed, or success message if succeeded
    /// </returns>
    /// <remarks>
    /// TODO [Database Required]: Implementation requires:
    /// - UserManager to create the user in the database
    /// - RoleManager to assign default "User" role
    /// - Email service to send verification email with token
    /// - Generate email confirmation token using UserManager.GenerateEmailConfirmationTokenAsync
    /// </remarks>
    Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model);

    /// <summary>
    /// Authenticates a user with email and password, requiring email verification.
    /// </summary>
    /// <param name="model">The login credentials including email, password, and remember me option.</param>
    /// <returns>
    /// A task containing a tuple with:
    /// - bool: Success status
    /// - string: Error message if failed, or success message if succeeded
    /// </returns>
    /// <remarks>
    /// TODO [Database Required]: Implementation requires:
    /// - UserManager to find user by email
    /// - Check EmailConfirmed property (must be true to allow login)
    /// - SignInManager.PasswordSignInAsync for authentication
    /// - Cookie-based authentication with lockout on failure
    /// - Update LastLoginAt timestamp on ApplicationUser
    /// </remarks>
    Task<(bool Success, string Message)> LoginAsync(LoginViewModel model);

    /// <summary>
    /// Verifies a user's email address using the confirmation token.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>
    /// A task containing a tuple with:
    /// - bool: Success status
    /// - string: Error message if failed, or success message if succeeded
    /// </returns>
    /// <remarks>
    /// TODO [Database Required]: Implementation requires:
    /// - UserManager to find user by userId
    /// - UserManager.ConfirmEmailAsync to verify token and mark email as confirmed
    /// - Set ApplicationUser.EmailConfirmed = true
    /// </remarks>
    Task<(bool Success, string Message)> VerifyEmailAsync(string userId, string token);

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    /// <returns>A task representing the asynchronous logout operation.</returns>
    /// <remarks>
    /// TODO [Database Required]: Implementation requires:
    /// - SignInManager.SignOutAsync to clear authentication cookies
    /// </remarks>
    Task LogoutAsync();

    // TODO [Future Enhancement]: Add password reset methods when required
    // Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordViewModel model);
    // Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordViewModel model);
}
