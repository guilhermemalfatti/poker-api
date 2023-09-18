using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FortisService.Core.Models.Tables;
using FortisService.Models.Models.Tables;

namespace FortisService.DataContext.EntityConfigurations
{
    /// <summary>
    /// Model builder configuration for <see cref="StatusHistory"/> entity type.
    /// </summary>
    internal class StatusHistoryEntityConfiguration : IEntityTypeConfiguration<StatusHistory>
    {
        public void Configure(EntityTypeBuilder<StatusHistory> builder)
        {
            builder
                .HasIndex(b => b.Id)
                .IsUnique();

            // not using because Sqlite dont support generated values on composite keys
            // builder.HasKey(sh => new { sh.Id, sh.PlayerId, sh.GameId });

            builder.HasOne(sh => sh.Game)
                .WithMany(g => g.StatusHistories)
                .HasForeignKey(u => u.GameId);

            builder.HasOne(sh => sh.Player)
                .WithMany(g => g.StatusHistories)
                .HasForeignKey(u => u.PlayerId);
        }
    }
}
