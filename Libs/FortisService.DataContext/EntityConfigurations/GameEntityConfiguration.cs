using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FortisService.Core.Models.Tables;

namespace FortisService.DataContext.EntityConfigurations
{
    /// <summary>
    /// Model builder configuration for <see cref="Game"/> entity type.
    /// </summary>
    internal class GameEntityConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasIndex(b => b.Id).IsUnique();
        }
    }
}
