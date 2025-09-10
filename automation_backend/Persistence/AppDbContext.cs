using Microsoft.EntityFrameworkCore;
using dotnet.Persistence.Entities;

namespace dotnet.Persistence
{
    /// <summary>
    /// EF Core DbContext for the management database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<SocialAccount> SocialAccounts => Set<SocialAccount>();
        public DbSet<ScheduledPost> ScheduledPosts => Set<ScheduledPost>();
        public DbSet<PublishedPost> PublishedPosts => Set<PublishedPost>();
        public DbSet<AnalyticsSnapshot> AnalyticsSnapshots => Set<AnalyticsSnapshot>();
        public DbSet<AutomationRule> AutomationRules => Set<AutomationRule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<SocialAccount>()
                .HasIndex(a => new { a.Provider, a.ProviderUserId, a.UserId })
                .IsUnique();

            modelBuilder.Entity<ScheduledPost>()
                .HasOne(p => p.SocialAccount)
                .WithMany(a => a.ScheduledPosts)
                .HasForeignKey(p => p.SocialAccountId);

            modelBuilder.Entity<PublishedPost>()
                .HasOne(p => p.SocialAccount)
                .WithMany(a => a.PublishedPosts)
                .HasForeignKey(p => p.SocialAccountId);

            modelBuilder.Entity<AutomationRule>()
                .HasOne(r => r.User)
                .WithMany(u => u.AutomationRules)
                .HasForeignKey(r => r.UserId);
        }
    }
}
