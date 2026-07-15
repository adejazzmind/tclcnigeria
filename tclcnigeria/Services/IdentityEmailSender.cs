using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using tclcnigeria.Services;

namespace tclcnigeria.Services
{
    // Handles Identity emails: password reset, email confirmation
    public class IdentityEmailSender : IEmailSender<IdentityUser>, IEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public IdentityEmailSender(IEmailService emailService, IConfiguration config)
        {
            _emailService = emailService;
            _config = config;
        }

        // Called when admin requests password reset
        public async Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>&#128274; Password Reset Request</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>TCLC Nigeria Admin</p>
              </div>
              <div style='padding:32px;'>
                <p style='color:#333;'>Hello,</p>
                <p style='color:#555;line-height:1.8;'>
                  We received a request to reset the password for your TCLC Nigeria admin account.
                  Click the button below to reset your password.
                </p>
                <div style='text-align:center;margin:32px 0;'>
                  <a href='{resetLink}'
                     style='background:#070D2E;color:#F5C842;padding:14px 36px;
                            border-radius:50px;text-decoration:none;font-weight:700;
                            font-size:1rem;display:inline-block;'>
                    Reset My Password
                  </a>
                </div>
                <p style='color:#888;font-size:.85rem;'>
                  This link will expire in 24 hours. If you did not request a password reset,
                  please ignore this email.
                </p>
                <p style='color:#888;font-size:.82rem;word-break:break-all;'>
                  Or copy this link: {resetLink}
                </p>
              </div>
              <div style='background:#070D2E;padding:16px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.4);font-size:.75rem;margin:0;'>
                  TCLC Nigeria &bull; 25, Kudirat Soule Street, Ifako, Ogba, Lagos
                </p>
              </div>
            </div>";

            await _emailService.SendEmailAsync(email, user.UserName ?? "Admin", "Reset Your TCLC Nigeria Password", html);
        }

        public async Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>Your Reset Code</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>TCLC Nigeria Admin</p>
              </div>
              <div style='padding:32px;'>
                <p style='color:#333;'>Your password reset code is:</p>
                <div style='background:#070D2E;border-radius:12px;padding:24px;text-align:center;margin:24px 0;'>
                  <p style='color:#F5C842;font-size:2rem;font-weight:700;letter-spacing:8px;margin:0;'>
                    {resetCode}
                  </p>
                </div>
                <p style='color:#888;font-size:.85rem;'>This code expires in 15 minutes.</p>
              </div>
              <div style='background:#070D2E;padding:16px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.4);font-size:.75rem;margin:0;'>TCLC Nigeria Admin System</p>
              </div>
            </div>";

            await _emailService.SendEmailAsync(email, user.UserName ?? "Admin", "Your TCLC Nigeria Reset Code", html);
        }

        public async Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
        {
            var html = $@"
            <div style='font-family:sans-serif;max-width:600px;margin:0 auto;background:#f9f9f9;border-radius:12px;overflow:hidden;'>
              <div style='background:#070D2E;padding:32px;text-align:center;'>
                <h1 style='color:#F5C842;font-size:1.4rem;margin:0;'>Confirm Your Email</h1>
                <p style='color:rgba(255,255,255,0.7);margin:8px 0 0;'>TCLC Nigeria</p>
              </div>
              <div style='padding:32px;'>
                <p style='color:#555;'>Please confirm your email address by clicking below:</p>
                <div style='text-align:center;margin:32px 0;'>
                  <a href='{confirmationLink}'
                     style='background:#070D2E;color:#F5C842;padding:14px 36px;
                            border-radius:50px;text-decoration:none;font-weight:700;display:inline-block;'>
                    Confirm Email
                  </a>
                </div>
              </div>
              <div style='background:#070D2E;padding:16px;text-align:center;'>
                <p style='color:rgba(255,255,255,0.4);font-size:.75rem;margin:0;'>TCLC Nigeria</p>
              </div>
            </div>";

            await _emailService.SendEmailAsync(email, user.UserName ?? "User", "Confirm Your TCLC Nigeria Email", html);
        }

        // IEmailSender (non-generic) implementation
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await _emailService.SendEmailAsync(email, "User", subject, htmlMessage);
        }
    }
}