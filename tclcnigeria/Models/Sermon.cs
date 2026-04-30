using System.ComponentModel.DataAnnotations;

namespace tclcnigeria.Models
{
    public class Sermon
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Speaker { get; set; }

        [DataType(DataType.Date)]
        public DateTime DatePreached { get; set; }

        public string Description { get; set; }

        [Url]
        public string VideoUrl { get; set; }

        public string MediaUrl { get; set; }

        public string SeriesName { get; set; }
    }
}