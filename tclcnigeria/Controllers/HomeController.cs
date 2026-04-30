using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using tclcnigeria.Models;

namespace tclcnigeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // --- 1. Constructor (Dependency Injection) ---
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // --- 2. Main Church Web Pages ---

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Ministries() => View();

        public IActionResult Sermons()
        {
            // PULL DATA: This fetches all sermons from your SQL Database
            var sermonList = _context.Sermons.OrderByDescending(s => s.DatePreached).ToList();
            return View(sermonList);
        }

        // GET: Displays the Contact Page
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Handles the form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                // SAVE DATA: This saves the contact message to your SQL Database
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Thank you! Your message has been recorded and we will reach out shortly.";
                return RedirectToAction("Contact");
            }
            return View(model);
        }

        // --- 3. System & Legal Pages ---

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}