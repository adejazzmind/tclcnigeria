using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class ExamAttempt
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public int PercentScore { get; set; }

        public bool Passed { get; set; }

        public DateTime SubmittedAt { get; set; }

        public bool RetakeApproved { get; set; } = false;

        public string? RetakeApprovedBy { get; set; }
    }
}
