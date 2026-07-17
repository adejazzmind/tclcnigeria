namespace tclcnigeria.Models
{
    public class Sermon
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Speaker { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? AudioUrl { get; set; }
        public string? MediaUrl { get; set; }
        public string? Description { get; set; }
        public string? SeriesName { get; set; }
        public DateTime DatePreached { get; set; } = DateTime.Today;
    }
}
