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

            builder.HasOne(sh => sh.FirstCard)
                .WithMany(c => c.FirstCards)
                .HasForeignKey(sh => sh.FirstCardId);

            builder.HasOne(sh => sh.SecondCard)
                .WithMany(c => c.SecondCards)
                .HasForeignKey(sh => sh.SecondCardId);

            builder.HasOne(sh => sh.ThirdCard)
                .WithMany(c => c.ThirdCards)
                .HasForeignKey(sh => sh.ThirdCardId);

            builder.HasOne(sh => sh.FourthCard)
                .WithMany(c => c.FourthCards)
                .HasForeignKey(sh => sh.FourthCardId);

            builder.HasOne(sh => sh.FifthCard)
                .WithMany(c => c.FifthCards)
                .HasForeignKey(sh => sh.FifthCardId);
        }
    }
}
