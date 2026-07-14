namespace tclcnigeria.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
        Task SendPrayerRequestNotificationAsync(string name, string email, string request);
        Task SendContactFormNotificationAsync(string name, string email, string subject, string message);
        Task SendEventReminderAsync(string toEmail, string toName, string eventTitle, string eventDate, string eventDescription);
    }
}
