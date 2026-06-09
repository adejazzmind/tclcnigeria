using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    // PUBLIC
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

    // ADMIN
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChurchEvent churchEvent)
        {
            if (ModelState.IsValid)
            {
                churchEvent.CreatedAt = DateTime.UtcNow;
                _context.Add(churchEvent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(churchEvent);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var churchEvent = await _context.ChurchEvents.FindAsync(id);
            if (churchEvent == null) return NotFound();
            return View(churchEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChurchEvent churchEvent)
        {
            if (id != churchEvent.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(churchEvent);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(churchEvent);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var churchEvent = await _context.ChurchEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (churchEvent == null) return NotFound();
            return View(churchEvent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var churchEvent = await _context.ChurchEvents.FindAsync(id);
            if (churchEvent != null) _context.ChurchEvents.Remove(churchEvent);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Event deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
