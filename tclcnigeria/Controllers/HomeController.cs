using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using tclcnigeria.Models;
using tclcnigeria.Services;

namespace tclcnigeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        // --- Main Pages ---
        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Ministries() => View();

        public IActionResult Sermons()
        {
            var sermonList = _context.Sermons.OrderByDescending(s => s.DatePreached).ToList();
            return View(sermonList);
        }

        // GET: Contact
        public IActionResult Contact() => View();

        // POST: Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                model.DateSent = DateTime.UtcNow;
                _context.ContactMessages.Add(model);
                _context.SaveChanges();

                // Send email notifications
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendContactFormNotificationAsync(
                            model.Name,
                            model.Email,
                            model.Subject,
                            model.Message
                        );
                    }
                    catch { /* silent fail — don't break user experience */ }
                });

                TempData["Success"] = "Thank you! Your message has been received and we will reach out shortly.";
                return RedirectToAction("Contact");
            }
            return View(model);
        }

        // --- System ---
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
