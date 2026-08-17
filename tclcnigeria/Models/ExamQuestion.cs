using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string OptionA { get; set; } = string.Empty;

        [Required]
        public string OptionB { get; set; } = string.Empty;

        public string? OptionC { get; set; }

        public string? OptionD { get; set; }

        // "A", "B", "C", or "D"
        [Required]
        public string CorrectOption { get; set; } = "A";

        public int Order { get; set; }
    }
}
