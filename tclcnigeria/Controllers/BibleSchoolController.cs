using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    public class BibleSchoolController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BibleSchoolController> _logger;

        public BibleSchoolController(ApplicationDbContext context, ILogger<BibleSchoolController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Index(BibleSchoolApplication model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.AppliedAt = DateTime.UtcNow;
                model.Status = "Pending";
                _context.BibleSchoolApplications.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.Success = "Thank you! Your Bible School application has been received. Our team will review it and reach out to you soon.";
                return View(new BibleSchoolApplication());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save Bible School application");
                ModelState.AddModelError("", "Something went wrong. Please try again.");
                return View(model);
            }
        }
    }

    [Authorize(Roles = "BibleSchoolStaff")]
    public class BibleSchoolAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BibleSchoolAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var applications = await _context.BibleSchoolApplications
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
            return View(applications);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var app = await _context.BibleSchoolApplications.FindAsync(id);
            if (app != null)
            {
                app.Status = "Approved";
                app.ReviewedBy = User.Identity?.Name;
                app.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var app = await _context.BibleSchoolApplications.FindAsync(id);
            if (app != null)
            {
                app.Status = "Rejected";
                app.ReviewedBy = User.Identity?.Name;
                app.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
