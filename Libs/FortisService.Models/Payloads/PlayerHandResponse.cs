using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace FortisService.Models.Payloads
{
    public class PlayerHandResponse
    {
        [JsonIgnore]
        public Player Player { get; set; }

        /// <summary>
        /// Rank by the highest hand card type
        /// </summary>
        /// [JsonIgnore]
        public Rank HandTypeRank { get; set; }

        public string PlayerName { get => Player.Name; }

        public bool IsWinner { get; set; } = false;

        [JsonIgnore]
        public PokerHandType? HandType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HandTypeName { get => HandType?.ToString();  }

        [JsonIgnore]
        public IList<Card> CardList { get; set; }

        /// <summary>
        /// List represents only the cards used to verify the high card hand
        /// </summary>
        [JsonIgnore]
        public IList<Card> HighCardList { get; set; }

        [JsonIgnore]
        public Card? HighCard { get => HighCardList?.OrderByDescending(c => c.Rank).FirstOrDefault(); }

        [JsonIgnore]
        public Card? HighCardByHandType { get; set;  }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string HightCardName { get => HighCard?.ToString(); }

        public IList<string> CardsName { get => CardList.Select(c => c.ToString()).ToList(); }

        public PlayerHandResponse()
        {
        }

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
