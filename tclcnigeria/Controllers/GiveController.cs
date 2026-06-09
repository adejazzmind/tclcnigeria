using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    // PUBLIC
    public class GiveController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GiveController(ApplicationDbContext context) => _context = context;

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordDonation(Donation model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.PaymentStatus = "Completed";
                _context.Donations.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Thank you {model.DonorName}! Your gift of {model.Amount:N0} has been received. God bless you!";
                return RedirectToAction(nameof(ThankYou));
            }
            return View("Index", model);
        }

        public IActionResult ThankYou() => View();
    }

    // ADMIN
    [Authorize]
    public class GivingAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GivingAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var donations = await _context.Donations
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            ViewBag.TotalThisMonth = donations
                .Where(d => d.CreatedAt.Month == DateTime.UtcNow.Month
                         && d.CreatedAt.Year == DateTime.UtcNow.Year
                         && d.PaymentStatus == "Completed")
                .Sum(d => d.Amount);

            ViewBag.TotalAllTime = donations
                .Where(d => d.PaymentStatus == "Completed")
                .Sum(d => d.Amount);

            ViewBag.DonorCount = donations
                .Where(d => d.PaymentStatus == "Completed")
                .Select(d => d.Email)
                .Distinct()
                .Count();

            return View(donations);
        }
    }
}
