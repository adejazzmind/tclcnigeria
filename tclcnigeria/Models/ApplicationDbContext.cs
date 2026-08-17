using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace tclcnigeria.Models
{
    public class ApplicationDbContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Sermon> Sermons { get; set; }
        public DbSet<ContactForm> ContactMessages { get; set; }
        public DbSet<ChurchEvent> ChurchEvents { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<BibleSchoolApplication> BibleSchoolApplications { get; set; }
        public DbSet<CtgEnrollment> CtgEnrollments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
    }
}


