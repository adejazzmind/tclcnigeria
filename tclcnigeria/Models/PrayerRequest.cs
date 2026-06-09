using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class PrayerRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your prayer request")]
        [StringLength(1000, MinimumLength = 10)]
        [Display(Name = "Prayer Request")]
        public string Request { get; set; } = string.Empty;

        [Display(Name = "Keep Anonymous")]
        public bool IsAnonymous { get; set; } = false;

        [Display(Name = "Prayed For")]
        public bool IsPrayedFor { get; set; } = false;

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }
    }
}
