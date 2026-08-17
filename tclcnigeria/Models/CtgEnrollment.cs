using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class CtgEnrollment
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string? DepartmentInterest { get; set; }

        // Pending, Approved, Rejected
        public string Status { get; set; } = "Pending";

        public DateTime AppliedAt { get; set; }

        public string? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
