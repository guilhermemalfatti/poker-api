using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortisService.Models.Enumerator
{

    public enum PokerHandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfKind,
        Straight,
        Flush,
        FullHouse,
        FourOfKind,
        StraightFlush,
        RoyalFlush
    }

    public enum Rank {
        Two, 
        Three,
        Four,
        Five, 
        Six,
        Seven,
        Eight, 
        Nine, 
        Ten,
        Jack, 
        Queen,
        King,
        Ace 
    }

    public enum Suit {
        Hearts,
        Diamonds,
        Clubs,
        Spades }
}
