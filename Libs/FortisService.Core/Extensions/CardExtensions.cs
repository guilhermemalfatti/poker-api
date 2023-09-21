using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Core.Extensions
{
    /// <summary>
    /// Extension methods to easy the process to find the highest hand
    /// </summary>
    public static class CardExtensions
    {

        public static bool IsRoyalFlush(this IList<Card> cards)
        {
            return cards.IsStraightFlush() && cards.Any(card => card.Rank == Rank.Ace);
        }

        public static bool IsStraightFlush(this IList<Card> cards)
        {
            return cards.IsFlush() && cards.IsStraight();
        }

        public static bool IsFourOfAKind(this IList<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 4);
        }

        public static bool IsFullHouse(this IList<Card> cards)
        {
            var groups = cards.GroupBy(card => card.Rank).ToList();
            return groups.Count == 2 && groups.Any(g => g.Count() == 3);
        }

        public static bool IsFlush(this IList<Card> cards)
        {
            var suit = cards.First().Suit;
            return cards.All(c => c.Suit == suit); 
        }

        public static bool IsStraight(this IList<Card> cards)
        {
            var distinctRanks = cards.Select(card => card.Rank).Distinct().OrderBy(c => c).ToList();

            // todo check the order
            if (distinctRanks.Count < 5)
                return false;

            for (int i = 0; i < distinctRanks.Count - 1; i++)
            {
                if (distinctRanks[i] - distinctRanks[i + 1] != -1)
                    return false;
            }

            return true;
        }

        public static bool IsThreeOfAKind(this IList<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 3);
        }

        public static bool IsTwoPair(this IList<Card> cards)
        {
            var groups = cards.GroupBy(card => card.Rank);
            return  groups.Count(group => group.Count() == 2) == 2;
        }

        public static bool IsOnePair(this IList<Card> cards)
        {
            return cards.GroupBy(card => card.Rank).Any(group => group.Count() == 2);
        }
    }
}
