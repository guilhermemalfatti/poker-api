using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortisService.Models.Payloads
{
    public class PlayerHandResponse
    {
        [JsonIgnore]
        public Player Player { get; set; }

        private string PlayerName { get => Player.Name; }

        public bool IsWinner { get; set; } = false;

        public PokerHandType? HandType { get; set; }

        [JsonIgnore]
        public IList<Card> CardList { get; set; }

        [JsonIgnore]
        public IList<Card> HighCardList { get; set; }

        public Card? HighCard { get => HighCardList?.OrderBy(c => c.Rank).FirstOrDefault(); }

        public IList<string> Cards { get => CardList.Select(c => c.ToString()).ToList(); }

        public PlayerHandResponse(Player player, IList<Card> cards)
        {
            Player = player;
            CardList = cards.OrderByDescending(c => c.Rank).ToList();
        }
        public PlayerHandResponse(Player player, IList<Card> cards, bool winner)
        {
            Player = player;
            CardList = cards.OrderByDescending(c => c.Rank).ToList();
            IsWinner = winner;
        }
        public PlayerHandResponse(Player player, IList<Card> cards, PokerHandType handType)
        {
            Player = player;
            CardList = cards.OrderByDescending(c => c.Rank).ToList();
            HandType = handType;
        }
    }
}
