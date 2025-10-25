using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers;

// Restrict entire controller to Admin role to prevent DOR/BOLA attacks
// Users must be authenticated AND have the Admin role to access any action
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{

    // GET: Admin Dashboard
    public IActionResult Index()
    {
        return View();
    }

    // GET: Manage Stories
    public IActionResult ManageStories()
    {
        return View();
    }

    // GET: Manage Events (placeholder)
    public IActionResult ManageEvents()
    {
        return View();
    }

    // GET: Manage Event Details (placeholder)
    public IActionResult ManageEventDetails()
    {
        return View();
    }

    // GET: Approve Volunteer Hours (placeholder)
    public IActionResult ApproveVolunteerHours()
    {
        return View();
    }

    // GET: Reports (placeholder)
    public IActionResult Reports()
    {
        return View();
    }
}
