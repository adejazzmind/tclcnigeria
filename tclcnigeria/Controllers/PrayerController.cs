using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;
using tclcnigeria.Services;

namespace tclcnigeria.Controllers
{
    public class PrayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<PrayerController> _logger;

        public PrayerController(ApplicationDbContext context, IEmailService emailService, ILogger<PrayerController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Index(PrayerRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.DateSubmitted = DateTime.UtcNow;
                _context.PrayerRequests.Add(model);
                await _context.SaveChangesAsync();

                var name = model.Name;
                var email = model.Email ?? string.Empty;
                var request = model.Request;

                _ = Task.Run(async () =>
                {
                    try { await _emailService.SendPrayerRequestNotificationAsync(name, email, request); }
                    catch (Exception ex) { _logger.LogError(ex, "Email failed for {Name}", name); }
                });

                ViewBag.Success = "Your prayer request has been received. Our intercessory team will pray with you for 30 days.";
                return View(new PrayerRequest());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save prayer request");
                ModelState.AddModelError("", "Something went wrong. Please try again.");
                return View(model);
            }
        }
    }

    [Authorize]
    public class PrayerAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PrayerAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var requests = await _context.PrayerRequests
                .OrderByDescending(p => p.DateSubmitted)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPrayed(int id)
        {
            var request = await _context.PrayerRequests.FindAsync(id);
            if (request != null)
            {
                request.IsPrayedFor = true;
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.PrayerRequests.FirstOrDefaultAsync(p => p.Id == id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.PrayerRequests.FindAsync(id);
            if (request != null) _context.PrayerRequests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
