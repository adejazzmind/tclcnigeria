using Microsoft.AspNetCore.Identity.UI.Services;
using tclcnigeria.Services;

namespace tclcnigeria.Services
{
    public class IdentityEmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<IdentityEmailSender> _logger;

        public IdentityEmailSender(IEmailService emailService, ILogger<IdentityEmailSender> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                await _emailService.SendEmailAsync(email, "User", subject, htmlMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send identity email to {Email}", email);
            }
        }
    }
}
