using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace tclcnigeria.Models
{
    public class ApplicationDbContext : IdentityDbContext
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
    }
}
