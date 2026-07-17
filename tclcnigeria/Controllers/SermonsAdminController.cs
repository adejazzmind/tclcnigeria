using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    [Authorize] // This locks the entire controller; only logged-in users can enter
    public class SermonsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SermonsAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SermonsAdmin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sermons.OrderByDescending(s => s.DatePreached).ToListAsync());
        }

        // GET: SermonsAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sermon = await _context.Sermons.FirstOrDefaultAsync(m => m.Id == id);

            if (sermon == null) return NotFound();

            return View(sermon);
        }

        // GET: SermonsAdmin/Create
        public IActionResult Create() => View();

        // POST: SermonsAdmin/Create
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Speaker,DatePreached,Description,VideoUrl,MediaUrl,SeriesName")] Sermon sermon)
        {
            if (ModelState.IsValid)
            {
                // Format YouTube link before saving
                sermon.VideoUrl = FormatYouTubeLink(sermon.VideoUrl);

                _context.Add(sermon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sermon);
        }

        // GET: SermonsAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sermon = await _context.Sermons.FindAsync(id);
            if (sermon == null) return NotFound();

            return View(sermon);
        }

        // POST: SermonsAdmin/Edit/5
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Speaker,DatePreached,Description,VideoUrl,MediaUrl,SeriesName")] Sermon sermon)
        {
            if (id != sermon.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Format YouTube link before updating
                    sermon.VideoUrl = FormatYouTubeLink(sermon.VideoUrl);

                    _context.Update(sermon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SermonExists(sermon.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sermon);
        }

        // GET: SermonsAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sermon = await _context.Sermons.FirstOrDefaultAsync(m => m.Id == id);
            if (sermon == null) return NotFound();

            return View(sermon);
        }

        // POST: SermonsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sermon = await _context.Sermons.FindAsync(id);
            if (sermon != null)
            {
                _context.Sermons.Remove(sermon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- Helper Methods ---

        private bool SermonExists(int id)
        {
            return _context.Sermons.Any(e => e.Id == id);
        }

        private string FormatYouTubeLink(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;

            if (url.Contains("watch?v="))
            {
                return url.Replace("watch?v=", "embed/");
            }
            return url;
        }
    }
}
