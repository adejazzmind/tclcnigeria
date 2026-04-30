using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Change this
using Microsoft.EntityFrameworkCore;

namespace tclcnigeria.Models
{
    // Change inheritance to IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Sermon> Sermons { get; set; }
        public DbSet<ContactForm> ContactMessages { get; set; }
    }
}