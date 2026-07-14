using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;
using tclcnigeria.Services;

namespace tclcnigeria.Controllers
{
    // PUBLIC
    public class PrayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PrayerController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PrayerRequest model)
        {
            if (ModelState.IsValid)
            {
                model.DateSubmitted = DateTime.UtcNow;
                _context.PrayerRequests.Add(model);
                await _context.SaveChangesAsync();

                // Send email notifications (fire and forget — doesn't block the user)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendPrayerRequestNotificationAsync(
                            model.Name,
                            model.Email ?? string.Empty,
                            model.Request
                        );
                    }
                    catch { /* log silently — don't break user experience */ }
                });

                TempData["Success"] = "Your prayer request has been received. Our team will pray with you.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }

    // ADMIN
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
