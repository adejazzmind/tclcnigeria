using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class Donation
    {
        public int Id { get; set; }

        [Required]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(100, 10000000, ErrorMessage = "Minimum donation is 100")]
        public decimal Amount { get; set; }

        [Display(Name = "Giving Type")]
        public string GivingType { get; set; } = "Tithe";

        public string? Note { get; set; }

        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Display(Name = "Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
