using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var events = await _context.ChurchEvents
                .Where(e => e.EventDate >= DateTime.UtcNow.AddDays(-1))
                .OrderBy(e => e.EventDate)
                .ToListAsync();
            return View(events);
        }
    }

    [Authorize]
    public class EventsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventsAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var events = await _context.ChurchEvents
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
            return View(events);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,EventDate,EventEndDate,Location,Category,ImageUrl,IsFeatured")] ChurchEvent churchEvent)
        {
            try
            {
                ModelState.Clear();
                churchEvent.EventDate = DateTime.SpecifyKind(
                    churchEvent.EventDate == default ? DateTime.UtcNow : churchEvent.EventDate,
                    DateTimeKind.Utc);
                if (churchEvent.EventEndDate.HasValue)
                    churchEvent.EventEndDate = DateTime.SpecifyKind(churchEvent.EventEndDate.Value, DateTimeKind.Utc);
                churchEvent.CreatedAt = DateTime.UtcNow;
                churchEvent.Title = churchEvent.Title ?? "Untitled";
                _context.Add(churchEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message} | {ex.InnerException?.Message}");
                return View(churchEvent);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _context.ChurchEvents.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDate,EventEndDate,Location,Category,ImageUrl,IsFeatured,CreatedAt")] ChurchEvent churchEvent)
        {
            if (id != churchEvent.Id) return NotFound();
            try
            {
                ModelState.Clear();
                churchEvent.EventDate = DateTime.SpecifyKind(churchEvent.EventDate, DateTimeKind.Utc);
                if (churchEvent.EventEndDate.HasValue)
                    churchEvent.EventEndDate = DateTime.SpecifyKind(churchEvent.EventEndDate.Value, DateTimeKind.Utc);
                _context.Update(churchEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message} | {ex.InnerException?.Message}");
                return View(churchEvent);
            }
        }

        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var ev = await _context.ChurchEvents.FindAsync(id);
            if (ev != null) _context.ChurchEvents.Remove(ev);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
