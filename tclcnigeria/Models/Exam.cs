using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int PassingScorePercent { get; set; } = 70;

        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public List<ExamQuestion> Questions { get; set; } = new();
    }
}
