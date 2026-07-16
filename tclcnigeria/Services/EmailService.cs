using System.Text;
using System.Text.Json;

namespace tclcnigeria.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var apiKey = _config["Email:BrevoApiKey"] ?? "";
                var fromEmail = _config["Email:SmtpUser"] ?? "adejazzmind@gmail.com";
                var fromName = _config["Email:FromName"] ?? "TCLC Nigeria";

                var payload = new { sender = new { name = fromName, email = fromEmail }, to = new[] { new { email = toEmail, name = toName } }, subject = subject, htmlContent = htmlBody };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("api-key", apiKey);
                client.DefaultRequestHeaders.Add("accept", "application/json");

                var response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Email sent to {Email}", toEmail);
                else
                    _logger.LogError("Brevo error: {Status} {Body}", response.StatusCode, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email failed to {Email}", toEmail);
            }
        }

        public async Task SendPrayerRequestNotificationAsync(string name, string email, string request)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? "adejazzmind@gmail.com";
            await SendEmailAsync(adminEmail, "TCLC Admin", $"New Prayer Request from {name}",
                $"<div style='font-family:sans-serif;padding:32px;background:#f9f9f9;'><h2 style='color:#070D2E;'>New Prayer Request</h2><p><strong>From:</strong> {name}</p><p><strong>Email:</strong> {email}</p><div style='background:#fff;border-left:4px solid #F5C842;padding:16px;'><p>{request}</p></div></div>");

            if (!string.IsNullOrEmpty(email))
                await SendEmailAsync(email, name, "Your Prayer Request - TCLC Nigeria",
                    $"<div style='font-family:sans-serif;padding:32px;background:#f9f9f9;'><div style='background:#070D2E;padding:24px;text-align:center;'><h2 style='color:#F5C842;margin:0;'>Prayer Request Received</h2></div><div style='padding:24px;'><p>Dear <strong>{name}</strong>,</p><p>Our intercessory team will pray with you for 30 days.</p><p><em>{request}</em></p><p>God bless,<br><strong>TCLC Nigeria</strong></p></div></div>");
        }

        public async Task SendContactFormNotificationAsync(string name, string email, string subject, string message)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? "adejazzmind@gmail.com";
            await SendEmailAsync(adminEmail, "TCLC Admin", $"New Message: {subject}",
                $"<div style='font-family:sans-serif;padding:32px;'><h2>New Contact Message</h2><p><strong>From:</strong> {name}</p><p><strong>Email:</strong> {email}</p><p><strong>Subject:</strong> {subject}</p><p>{message}</p></div>");
            await SendEmailAsync(email, name, "Message Received - TCLC Nigeria",
                $"<div style='font-family:sans-serif;padding:32px;'><h2 style='color:#070D2E;'>Message Received</h2><p>Dear {name},</p><p>We received your message and will respond within 24-48 hours.</p><p>God bless,<br><strong>TCLC Nigeria</strong></p></div>");
        }

        public async Task SendEventReminderAsync(string toEmail, string toName, string eventTitle, string eventDate, string eventDescription)
        {
            await SendEmailAsync(toEmail, toName, $"Reminder: {eventTitle}",
                $"<div style='font-family:sans-serif;padding:32px;'><h2>{eventTitle}</h2><p><strong>Date:</strong> {eventDate}</p><p>{eventDescription}</p><p>God bless,<br><strong>TCLC Nigeria</strong></p></div>");
        }
    }
}
