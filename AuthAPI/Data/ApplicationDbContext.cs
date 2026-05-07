using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<SoftwareUser> SoftwareUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<License>()
                .HasIndex(l => l.Key)
                .IsUnique();

            modelBuilder.Entity<SoftwareUser>()
                .HasIndex(u => new { u.Username, u.ApplicationId })
                .IsUnique();
        }
    }
}
