using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FortisService.Core.Models.Tables;
using FortisService.Models.Models.Tables;

namespace FortisService.DataContext.EntityConfigurations
{
    /// <summary>
    /// Model builder configuration for <see cref="Player"/> entity type.
    /// </summary>
    internal class PlayerEntityConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder
                .HasIndex(b => b.Id)
                .IsUnique();

            builder
                .HasIndex(b => b.Name)
                .IsUnique();
        }
    }
}
