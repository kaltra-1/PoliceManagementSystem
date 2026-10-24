using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<Suspect> Suspects { get; set; }
        public DbSet<Arrest> Arrests { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<ForensicReport> ForensicReports { get; set; }
        public DbSet<CitizenReport> CitizenReports { get; set; }
        // New tables
        public DbSet<Witness> Witnesses { get; set; }
        public DbSet<Patrol> Patrols { get; set; }
        public DbSet<MissingPersonReport> MissingPersonReports { get; set; }
        public DbSet<DrivingLicense> DrivingLicenses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        // New tables for full requirements coverage
        public DbSet<Department> Departments { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fine>()
                .Property(f => f.Amount)
                .HasPrecision(10, 2);

            // User: index on BadgeNumber for lockout lookup
            modelBuilder.Entity<User>()
                .HasIndex(u => u.BadgeNumber);

            // Setting: Key must be unique
            modelBuilder.Entity<Setting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            // Department: Code must be unique
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Notification: index by recipient for fast lookup
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.RecipientBadge, n.IsRead });
        }
    }
}
