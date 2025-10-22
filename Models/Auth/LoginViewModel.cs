using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Auth;

/// <summary>
/// View model for user login with email and password authentication.
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email address is required")]
    [Display(Name = "Email Address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; } = false;
}
