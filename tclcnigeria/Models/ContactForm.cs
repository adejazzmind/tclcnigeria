using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class ContactForm
    {
        // Primary Key for Database
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message")]
        [StringLength(500, MinimumLength = 10)]
        public string Message { get; set; }

        // --- Pro Admin Features ---

        [Display(Name = "Status")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Date Received")]
        public DateTime DateSent { get; set; } = DateTime.Now;
    }
}