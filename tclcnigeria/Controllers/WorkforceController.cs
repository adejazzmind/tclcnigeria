using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    public class WorkforceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WorkforceController> _logger;

        public WorkforceController(ApplicationDbContext context, ILogger<WorkforceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index() => View();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Index(CtgEnrollment model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.AppliedAt = DateTime.UtcNow;
                model.Status = "Pending";
                _context.CtgEnrollments.Add(model);
                await _context.SaveChangesAsync();

                ViewBag.Success = "Thank you! Your Workforce Discipleship enrollment has been received. You'll be notified once approved.";
                return View(new CtgEnrollment());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save CTG enrollment");
                ModelState.AddModelError("", "Something went wrong. Please try again.");
                return View(model);
            }
        }
    }

    [Authorize(Roles = "CTGStaff")]
    public class WorkforceAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public WorkforceAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.CtgEnrollments
                .OrderByDescending(e => e.AppliedAt)
                .ToListAsync();
            return View(enrollments);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var e = await _context.CtgEnrollments.FindAsync(id);
            if (e != null)
            {
                e.Status = "Approved";
                e.ReviewedBy = User.Identity?.Name;
                e.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var e = await _context.CtgEnrollments.FindAsync(id);
            if (e != null)
            {
                e.Status = "Rejected";
                e.ReviewedBy = User.Identity?.Name;
                e.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
