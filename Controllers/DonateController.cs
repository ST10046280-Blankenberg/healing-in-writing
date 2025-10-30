using System.Threading.Tasks;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Inject a donations service responsible for payment and receipt handling.
    public class DonateController : Controller
    {
        private readonly IBankDetailsService _bankDetailsService;

        public DonateController(IBankDetailsService bankDetailsService)
        {
            _bankDetailsService = bankDetailsService;
        }

        // TODO: Orchestrate donation flows by delegating to the donations service.
        public async Task<IActionResult> Index()
        {
            var bankDetails = await _bankDetailsService.GetAsync();
            return View(bankDetails.ToViewModel());
        }
    }
}
