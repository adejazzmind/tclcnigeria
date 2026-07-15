using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace tclcnigeria.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _config["Email:FromName"] ?? "TCLC Nigeria",
                    _config["Email:SmtpUser"]
                ));
                email.To.Add(new MailboxAddress(toName, toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlBody };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                var port = int.Parse(_config["Email:SmtpPort"] ?? "465");
                var useSsl = port == 465;
                await smtp.ConnectAsync(
                    _config["Email:SmtpHost"],
                    port,
                    useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls
                );
                await smtp.AuthenticateAsync(
                    _config["Email:SmtpUser"],
                    _config["Email:SmtpPass"]
                );
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        public async Task SendPrayerRequestNotificationAsync(string name, string email, string request)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? _config["Email:SmtpUser"];

            var adminHtml = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>&#128591; New Prayer Request</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;font-size:.9rem;'>TCLC Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p style='color:#333;'><strong>From:</strong> {name}</p>
                <p style='color:#333;'><strong>Email:</strong> {email}</p>
                <div style='background:#fff;border-left:4px solid #F5C842;padding:16px;border-radius:4px;margin-top:16px;'>
                  <p style='color:#555;margin:0;line-height:1.8;'>{request}</p>
                </div>
                <p style='color:#888;font-size:.85rem;margin-top:24px;'>
                  Please remember to pray for this request and follow up with the person.
                </p>
              </div>
              <div style='background:#070D2E;padding:16px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.4);font-size:.75rem;margin:0;'>TCLC Nigeria Admin Notification</p>
              </div>
            </div>";

            await SendEmailAsync(adminEmail!, "TCLC Admin", $"New Prayer Request from {name}", adminHtml);

            var userHtml = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>Your Prayer Request Has Been Received</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;font-size:.9rem;'>The City of the Lord Church Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p style='color:#333;'>Dear <strong>{name}</strong>,</p>
                <p style='color:#555;line-height:1.8;'>
                  Thank you for trusting us with your prayer request. Our intercessory team has received it
                  and will be standing in agreement with you in prayer for <strong>30 days</strong>.
                </p>
                <div style='background:#fff;border-left:4px solid #F5C842;padding:16px;border-radius:4px;margin:24px 0;'>
                  <p style='color:#555;margin:0;font-style:italic;line-height:1.8;'>&ldquo;{request}&rdquo;</p>
                </div>
                <p style='color:#555;line-height:1.8;'>
                  <strong>Matthew 18:20</strong> &mdash; &ldquo;For where two or three gather in my name,
                  there am I with them.&rdquo;
                </p>
                <p style='color:#555;line-height:1.8;'>God bless you,<br><strong>TCLC Nigeria Intercessory Team</strong></p>
              </div>
              <div style='background:#070D2E;padding:24px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.5);font-size:.78rem;margin:0;'>
                  25, Kudirat Soule Street, Ifako, Ogba, Lagos &bull; 07082768005
                </p>
              </div>
            </div>";

            await SendEmailAsync(email, name, "We have received your Prayer Request - TCLC Nigeria", userHtml);
        }

        public async Task SendContactFormNotificationAsync(string name, string email, string subject, string message)
        {
            var adminEmail = _config["Email:AdminEmail"] ?? _config["Email:SmtpUser"];

            var adminHtml = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>&#128232; New Contact Message</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>TCLC Nigeria Website</p>
              </div>
              <div style='padding:32px;'>
                <p><strong>From:</strong> {name}</p>
                <p><strong>Email:</strong> <a href='mailto:{email}'>{email}</a></p>
                <p><strong>Subject:</strong> {subject}</p>
                <div style='background:#fff;border-left:4px solid #1A56DB;padding:16px;border-radius:4px;margin-top:16px;'>
                  <p style='color:#555;margin:0;line-height:1.8;'>{message}</p>
                </div>
              </div>
              <div style='background:#070D2E;padding:16px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.4);font-size:.75rem;margin:0;'>TCLC Nigeria Contact Notification</p>
              </div>
            </div>";

            await SendEmailAsync(adminEmail!, "TCLC Admin", $"New Message: {subject} - from {name}", adminHtml);

            var userHtml = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>Message Received!</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>The City of the Lord Church Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p>Dear <strong>{name}</strong>,</p>
                <p style='color:#555;line-height:1.8;'>
                  Thank you for reaching out to us. We have received your message and will
                  respond to you as soon as possible, usually within 24-48 hours.
                </p>
                <p style='color:#555;line-height:1.8;'>God bless you,<br><strong>TCLC Nigeria Team</strong></p>
              </div>
              <div style='background:#070D2E;padding:24px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.5);font-size:.78rem;margin:0;'>
                  25, Kudirat Soule Street, Ifako, Ogba, Lagos &bull; 07082768005
                </p>
              </div>
            </div>";

            await SendEmailAsync(email, name, "We Got Your Message - TCLC Nigeria", userHtml);
        }

        public async Task SendEventReminderAsync(string toEmail, string toName, string eventTitle, string eventDate, string eventDescription)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>&#128197; Upcoming Event Reminder</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>The City of the Lord Church Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p>Dear <strong>{toName}</strong>,</p>
                <p style='color:#555;line-height:1.8;'>This is a reminder about an upcoming event at TCLC Nigeria:</p>
                <div style='background:#fff;border-radius:12px;padding:24px;border-top:4px solid #F5C842;margin:24px 0;'>
                  <h2 style='color:#070D2E;margin:0 0 8px;font-size:1.3rem;'>{eventTitle}</h2>
                  <p style='color:#1A56DB;font-weight:600;margin:0 0 12px;'>&#128197; {eventDate}</p>
                  <p style='color:#555;margin:0;line-height:1.8;'>{eventDescription}</p>
                </div>
                <p style='color:#555;'><strong>Location:</strong> 25, Kudirat Soule Street, Ifako, Ogba, Lagos</p>
                <p style='color:#555;line-height:1.8;'>We look forward to seeing you there!</p>
                <p style='color:#555;'>God bless,<br><strong>TCLC Nigeria</strong></p>
              </div>
              <div style='background:#070D2E;padding:24px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.5);font-size:.78rem;margin:0;'>
                  25, Kudirat Soule Street, Ifako, Ogba, Lagos &bull; 07082768005
                </p>
              </div>
            </div>";

            await SendEmailAsync(toEmail, toName, $"Reminder: {eventTitle} - TCLC Nigeria", html);
        }
    }
}
