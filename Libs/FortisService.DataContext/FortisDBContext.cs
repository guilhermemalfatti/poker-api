using FortisService.Core.Abstractions;
using FortisService.Core.Models.Tables;
using FortisService.DataContext.EntityConfigurations;
using FortisService.Models.Models.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FortisService.DataContext
{

    // This DB context intentionally keeps things simple for clarity
    public class FortisDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public FortisDbContext(DbContextOptions<FortisDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public DbSet<Game> Games { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<StatusHistory> StatusHistories { get; set; }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.LastUpdatedAt = now;
                            break;

                        case EntityState.Added:
                            trackable.CreatedAt = now;
                            trackable.LastUpdatedAt = now;
                            break;
                    }
                }
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

            modelBuilder.ApplyConfiguration(new GameEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CardEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StatusHistoryEntityConfiguration());

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(Configuration.GetConnectionString("database"));
    }
}
