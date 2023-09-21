using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FortisService.Core.Extensions
{
    public static class PlayerHandResponseExtension
    {
        public static Card GetGetHighestCardByHandType(this PlayerHandResponse playerResponse)
        {
            var card = new Card();
            switch (playerResponse.HandType)
            {
                case PokerHandType.FullHouse:
                    {
                        // todo test against the rank of is pairs if the triplet are the same
                        // the rule: Each full house is ranked first by the rank of its triplet, and then by the rank of its pair
                        var group = playerResponse.CardList.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).FirstOrDefault();
                        card = group.FirstOrDefault();
                        break;
                    }
                case PokerHandType.RoyalFlush:
                case PokerHandType.Flush:
                case PokerHandType.Straight:
                    {
                        card = playerResponse.CardList.OrderByDescending(c => c.Rank).FirstOrDefault();
                        break;
                    }
                case PokerHandType.StraightFlush:
                case PokerHandType.FourOfKind:
                case PokerHandType.ThreeOfKind:
                case PokerHandType.TwoPair:
                case PokerHandType.Pair:
                    {
                        var group = playerResponse.CardList.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).FirstOrDefault();
                        card = group.First();
                        break;
                    }
                default:
                    return null;
            }

            return card;
        }

        // it ignore the cards that bellongs to a hand type
        public static IList<Card> GetHighCardList(this PlayerHandResponse playerResponse)
        {
            var cards = new List<Card>();
            switch (playerResponse.HandType)
            {
                case PokerHandType.FullHouse:
                    {
                        // review it, we are only considerid the triplet here....
                        var group = playerResponse.CardList.GroupBy(c => c.Rank).OrderByDescending(c => c.Key).FirstOrDefault();
                        cards = group.ToList();
                        break;
                    }
                case PokerHandType.RoyalFlush:// todo review it
                case PokerHandType.Flush:// todo review it
                case PokerHandType.Straight:
                case PokerHandType.StraightFlush:
                    {
                        cards = playerResponse.CardList.ToList();
                        break;
                    }
                case PokerHandType.FourOfKind:
                case PokerHandType.ThreeOfKind:
                case PokerHandType.TwoPair:
                case PokerHandType.Pair:
                    {
                        var group = playerResponse.CardList.GroupBy(c => c.Rank).Where(g => g.Count() == 1);
                        cards = group.Select(g => g.First()).ToList();
                        break;
                    }
                default:
                    return null;
            }

            return cards;
        }


    }

}
