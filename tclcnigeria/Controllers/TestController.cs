using Microsoft.AspNetCore.Mvc;
using tclcnigeria.Services;

namespace tclcnigeria.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailService _emailService;
        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> Email()
        {
            try
            {
                await _emailService.SendEmailAsync(
                    "adejazzmind@gmail.com",
                    "Admin",
                    "TCLC Nigeria Email Test",
                    "<h1>Email is working!</h1><p>TCLC Nigeria email system is configured correctly.</p>"
                );
                return Content("Email sent successfully! Check your Gmail.");
            }
            catch (Exception ex)
            {
                return Content($"Email failed: {ex.Message}");
            }
        }
    }
}
