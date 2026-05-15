using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class ChurchEvent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Event Date & Time")]
        public DateTime EventDate { get; set; }

        [Display(Name = "End Date & Time")]
        public DateTime? EventEndDate { get; set; }

        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = "General"; // General, Youth, Women, Men, Outreach

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Date Created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}