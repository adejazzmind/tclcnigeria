using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    [Authorize]
    public class SermonsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SermonsAdminController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var sermons = await _context.Sermons.OrderByDescending(s => s.DatePreached).ToListAsync();
            return View(sermons);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Speaker,DatePreached,Description,VideoUrl,MediaUrl,AudioUrl,SeriesName")] Sermon sermon)
        {
            try
            {
                if (!string.IsNullOrEmpty(sermon.VideoUrl))
                    sermon.VideoUrl = ConvertToEmbed(sermon.VideoUrl);
                if (!string.IsNullOrEmpty(sermon.MediaUrl))
                    sermon.MediaUrl = ConvertToEmbed(sermon.MediaUrl);
                if (sermon.DatePreached == default)
                    sermon.DatePreached = DateTime.SpecifyKind(sermon.DatePreached == default ? DateTime.UtcNow : sermon.DatePreached, DateTimeKind.Utc);
                sermon.Title = sermon.Title ?? "";
                sermon.Speaker = sermon.Speaker ?? "";

                _context.Add(sermon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? "No inner exception";
                ModelState.AddModelError("", $"Error: {ex.Message} | Inner: {innerMsg}");
                return View(sermon);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sermon = await _context.Sermons.FindAsync(id);
            if (sermon == null) return NotFound();
            return View(sermon);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Speaker,DatePreached,Description,VideoUrl,MediaUrl,AudioUrl,SeriesName")] Sermon sermon)
        {
            if (id != sermon.Id) return NotFound();
            try
            {
                if (!string.IsNullOrEmpty(sermon.VideoUrl))
                    sermon.VideoUrl = ConvertToEmbed(sermon.VideoUrl);
                _context.Update(sermon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}");
                return View(sermon);
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sermon = await _context.Sermons.FindAsync(id);
            if (sermon != null) _context.Sermons.Remove(sermon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string ConvertToEmbed(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            if (url.Contains("youtu.be/"))
            {
                var videoId = url.Split("youtu.be/")[1].Split("?")[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }
            if (url.Contains("watch?v="))
            {
                var videoId = url.Split("watch?v=")[1].Split("&")[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }
            return url;
        }
    }
}
