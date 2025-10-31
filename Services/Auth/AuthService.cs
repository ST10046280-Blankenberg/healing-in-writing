using HealingInWriting.Data;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace HealingInWriting.Services.Auth;

/// <summary>
/// Service implementation for handling authentication operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthService> logger,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Registers a new user with email verification required.
    /// </summary>
    public async Task<(bool Success, string Message)> RegisterAsync(RegisterViewModel model)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "An account with this email address already exists.");
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = false, // Require email verification
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed for {Email}: {Errors}", model.Email, errors);
                return (false, $"Registration failed: {errors}");
            }

            // Assign default "User" role
            await _userManager.AddToRoleAsync(user, "User");

            // Create UserProfile for the new user
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Bio = string.Empty,
                City = string.Empty
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            // TODO [Future Enhancement]: Send email verification
            // For now, we'll auto-confirm email for testing purposes
            // Uncomment below to require email verification:
            // var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // // Send email with verification link containing userId and token
            // return (true, "Registration successful. Please check your email to verify your account.");

            // Temporarily auto-confirm email for development
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, confirmToken);

            _logger.LogInformation("User {Email} registered successfully with ProfileId {ProfileId}", model.Email, userProfile.ProfileId);
            return (true, "Registration successful. You can now log in.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", model.Email);
            return (false, "An error occurred during registration. Please try again.");
        }
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    public async Task<(bool Success, string Message)> LoginAsync(LoginViewModel model)
    {
        try
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", model.Email);
                return (false, "Invalid email or password.");
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login attempt with unconfirmed email: {Email}", model.Email);
                return (false, "Please verify your email address before logging in.");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive account: {Email}", model.Email);
                return (false, "Your account has been deactivated. Please contact support.");
            }

            // Attempt sign in
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Update last login timestamp
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User {Email} logged in successfully", model.Email);
                return (true, "Login successful.");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out: {Email}", model.Email);
                return (false, "Your account has been locked due to multiple failed login attempts. Please try again later.");
            }

            if (result.RequiresTwoFactor)
            {
                // TODO [Future Enhancement]: Implement two-factor authentication
                return (false, "Two-factor authentication is required.");
            }

            _logger.LogWarning("Failed login attempt for {Email}", model.Email);
            return (false, "Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", model.Email);
            return (false, "An error occurred during login. Please try again.");
        }
    }

    /// <summary>
    /// Verifies a user's email address using the confirmation token.
    /// </summary>
    public async Task<(bool Success, string Message)> VerifyEmailAsync(string userId, string token)
    {
        try
        {
            // Find user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Email verification attempt for non-existent user ID: {UserId}", userId);
                return (false, "Invalid verification link.");
            }

            // Check if already confirmed
            if (user.EmailConfirmed)
            {
                return (true, "Email address already verified. You can log in.");
            }

            // Confirm email
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                _logger.LogInformation("Email verified for user {Email}", user.Email);
                return (true, "Email verified successfully. You can now log in.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Email verification failed for user {Email}: {Errors}", user.Email, errors);
            return (false, "Invalid or expired verification link.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification for user ID: {UserId}", userId);
            return (false, "An error occurred during email verification. Please try again.");
        }
    }

    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            throw;
        }
    }
}
