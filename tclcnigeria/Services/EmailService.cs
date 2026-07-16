using System.Text;
using System.Text.Json;

namespace tclcnigeria.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var apiKey = _config["Email:BrevoApiKey"] ?? "";
                var fromEmail = _config["Email:SmtpUser"] ?? "adejazzmind@gmail.com";
                var fromName = _config["Email:FromName"] ?? "TCLC Nigeria";

                var payload = new
                {
                    sender = new { name = fromName, email = fromEmail },
                    to = new[] { new { email = toEmail, name = toName } },
                    subject = subject,
                    htmlContent = htmlBody
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
                _httpClient.DefaultRequestHeaders.Add("accept", "application/json");

                var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Email sent via Brevo API to {Email}", toEmail);
                else
                    _logger.LogError("Brevo API error: {Status} {Body}", response.StatusCode, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        public async Task SendPrayerRequestNotificationAsync(string name, string email, string request)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? _config["Email:SmtpUser"] ?? "";

            var adminHtml = $@"<div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>New Prayer Request</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>TCLC Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p><strong>From:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <div style='background:#fff;border-left:4px solid #F5C842;padding:16px;border-radius:4px;margin-top:16px;'>
                  <p style='color:#555;margin:0;line-height:1.8;'>{request}</p>
                </div>
              </div>
            </div>";

            await SendEmailAsync(adminEmail, "TCLC Admin", $"New Prayer Request from {name}", adminHtml);

            if (!string.IsNullOrEmpty(email))
            {
                var userHtml = $@"<div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
                  <div style='background:#070D2E;padding:32px;text-align:center;'>
                    <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>Prayer Request Received</h1>
                    <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>The City of the Lord Church Nigeria</p>
                  </div>
                  <div style='padding:32px;'>
                    <p>Dear <strong>{name}</strong>,</p>
                    <p style='color:#555;line-height:1.8;'>Thank you for trusting us with your prayer request. Our intercessory team will pray with you for <strong>30 days</strong>.</p>
                    <div style='background:#fff;border-left:4px solid #F5C842;padding:16px;border-radius:4px;margin:24px 0;'>
                      <p style='color:#555;margin:0;font-style:italic;'>{request}</p>
                    </div>
                    <p style='color:#555;'>God bless you,<br><strong>TCLC Nigeria Intercessory Team</strong></p>
                  </div>
                </div>";
                await SendEmailAsync(email, name, "We received your Prayer Request - TCLC Nigeria", userHtml);
            }
        }

        public async Task SendContactFormNotificationAsync(string name, string email, string subject, string message)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? _config["Email:SmtpUser"] ?? "";

            var adminHtml = $@"<div style='font-family:sans-serif;max-width:600px;margin:0 auto;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;margin:0;'>New Contact Message</h1>
              </div>
              <div style='padding:32px;'>
                <p><strong>From:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Subject:</strong> {subject}</p>
                <div style='background:#fff;border-left:4px solid #1A56DB;padding:16px;border-radius:4px;'>
                  <p style='color:#555;margin:0;'>{message}</p>
                </div>
              </div>
            </div>";
            await SendEmailAsync(adminEmail, "TCLC Admin", $"New Message: {subject}", adminHtml);

            var userHtml = $@"<div style='font-family:sans-serif;max-width:600px;margin:0 auto;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;margin:0;'>Message Received</h1>
              </div>
              <div style='padding:32px;'>
                <p>Dear <strong>{name}</strong>,</p>
                <p style='color:#555;'>We received your message and will respond within 24-48 hours.</p>
                <p style='color:#555;'>God bless,<br><strong>TCLC Nigeria</strong></p>
              </div>
            </div>";
            await SendEmailAsync(email, name, "We Got Your Message - TCLC Nigeria", userHtml);
        }

        public async Task SendEventReminderAsync(string toEmail, string toName, string eventTitle, string eventDate, string eventDescription)
        {
            var html = $@"<div style='font-family:sans-serif;max-width:600px;margin:0 auto;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;margin:0;'>Event Reminder</h1>
              </div>
              <div style='padding:32px;'>
                <p>Dear <strong>{toName}</strong>,</p>
                <h2 style='color:#070D2E;'>{eventTitle}</h2>
                <p style='color:#1A56DB;font-weight:600;'>{eventDate}</p>
                <p style='color:#555;'>{eventDescription}</p>
                <p>God bless,<br><strong>TCLC Nigeria</strong></p>
              </div>
            </div>";
            await SendEmailAsync(toEmail, toName, $"Reminder: {eventTitle}", html);
        }
    }
}
