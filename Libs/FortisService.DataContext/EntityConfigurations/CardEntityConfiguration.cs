using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FortisService.Core.Models.Tables;
using FortisService.Models.Models.Tables;
using FortisService.Models.Enumerator;

namespace FortisService.DataContext.EntityConfigurations
{
    /// <summary>
    /// Model builder configuration for <see cref="Card"/> entity type.
    /// </summary>
    internal class CardEntityConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder
                .HasIndex(b => b.Id)
                .IsUnique();

            builder.HasKey(c => new { c.Id, c.Rank, c.Suit });

            // initial seed data
            builder.HasData(
                new { Id = 1, Rank = Rank.Two, Suit = Suit.Hearts },
                new { Id = 2, Rank = Rank.Three, Suit = Suit.Hearts },
                new { Id = 3, Rank = Rank.Four, Suit = Suit.Hearts },
                new { Id = 4, Rank = Rank.Five, Suit = Suit.Hearts },
                new { Id = 5, Rank = Rank.Six, Suit = Suit.Hearts },
                new { Id = 6, Rank = Rank.Seven, Suit = Suit.Hearts },
                new { Id = 7, Rank = Rank.Eight, Suit = Suit.Hearts },
                new { Id = 8, Rank = Rank.Nine, Suit = Suit.Hearts },
                new { Id = 9, Rank = Rank.Ten, Suit = Suit.Hearts },
                new { Id = 10, Rank = Rank.Jack, Suit = Suit.Hearts },
                new { Id = 11, Rank = Rank.Queen, Suit = Suit.Hearts },
                new { Id = 12, Rank = Rank.King, Suit = Suit.Hearts },
                new { Id = 13, Rank = Rank.Ace, Suit = Suit.Hearts },

                new { Id = 14, Rank = Rank.Two, Suit = Suit.Clubs },
                new { Id = 15, Rank = Rank.Three, Suit = Suit.Clubs },
                new { Id = 16, Rank = Rank.Four, Suit = Suit.Clubs },
                new { Id = 17, Rank = Rank.Five, Suit = Suit.Clubs },
                new { Id = 18, Rank = Rank.Six, Suit = Suit.Clubs },
                new { Id = 19, Rank = Rank.Seven, Suit = Suit.Clubs },
                new { Id = 20, Rank = Rank.Eight, Suit = Suit.Clubs },
                new { Id = 21, Rank = Rank.Nine, Suit = Suit.Clubs },
                new { Id = 22, Rank = Rank.Ten, Suit = Suit.Clubs },
                new { Id = 23, Rank = Rank.Jack, Suit = Suit.Clubs },
                new { Id = 24, Rank = Rank.Queen, Suit = Suit.Clubs },
                new { Id = 25, Rank = Rank.King, Suit = Suit.Clubs },
                new { Id = 26, Rank = Rank.Ace, Suit = Suit.Clubs },

                new { Id = 27, Rank = Rank.Two, Suit = Suit.Spades },
                new { Id = 28, Rank = Rank.Three, Suit = Suit.Spades },
                new { Id = 29, Rank = Rank.Four, Suit = Suit.Spades },
                new { Id = 30, Rank = Rank.Five, Suit = Suit.Spades },
                new { Id = 31, Rank = Rank.Six, Suit = Suit.Spades },
                new { Id = 32, Rank = Rank.Seven, Suit = Suit.Spades },
                new { Id = 33, Rank = Rank.Eight, Suit = Suit.Spades },
                new { Id = 34, Rank = Rank.Nine, Suit = Suit.Spades },
                new { Id = 35, Rank = Rank.Ten, Suit = Suit.Spades },
                new { Id = 36, Rank = Rank.Jack, Suit = Suit.Spades },
                new { Id = 37, Rank = Rank.Queen, Suit = Suit.Spades },
                new { Id = 38, Rank = Rank.King, Suit = Suit.Spades },
                new { Id = 39, Rank = Rank.Ace, Suit = Suit.Spades },

                new { Id = 40, Rank = Rank.Two, Suit = Suit.Diamonds },
                new { Id = 41, Rank = Rank.Three, Suit = Suit.Diamonds },
                new { Id = 42, Rank = Rank.Four, Suit = Suit.Diamonds },
                new { Id = 43, Rank = Rank.Five, Suit = Suit.Diamonds },
                new { Id = 44, Rank = Rank.Six, Suit = Suit.Diamonds },
                new { Id = 45, Rank = Rank.Seven, Suit = Suit.Diamonds },
                new { Id = 46, Rank = Rank.Eight, Suit = Suit.Diamonds },
                new { Id = 47, Rank = Rank.Nine, Suit = Suit.Diamonds },
                new { Id = 48, Rank = Rank.Ten, Suit = Suit.Diamonds },
                new { Id = 49, Rank = Rank.Jack, Suit = Suit.Diamonds },
                new { Id = 50, Rank = Rank.Queen, Suit = Suit.Diamonds },
                new { Id = 51, Rank = Rank.King, Suit = Suit.Diamonds },
                new { Id = 52, Rank = Rank.Ace, Suit = Suit.Diamonds });
        }
    }
}
