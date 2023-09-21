using FortisService.Core.Extensions;
using FortisService.Core.Models.Tables;
using FortisService.Core.Services;
using FortisService.DataContext;
using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using NUnit.Framework;

namespace Fortis.UnitTests
{
    [TestFixture]
    public class GameServiceTests
    {
        private IList<Card> _cardsPair;

        [SetUp]
        public void SetUp()
        {
            _cardsPair = new List<Card> {
                new Card{
                    Rank = Rank.Ace,
                    Suit = Suit.Hearts
                },
                new Card{
                    Rank = Rank.Two,
                    Suit = Suit.Hearts
                },
                new Card{
                    Rank = Rank.Ten,
                    Suit = Suit.Hearts
                },
                new Card{
                    Rank = Rank.Five,
                    Suit = Suit.Hearts
                },
                new Card{
                    Rank = Rank.Five,
                    Suit = Suit.Diamonds
                },
            };
        }

        [Test]
        public void TestCardsPair_OK()
        {
            Assert.IsTrue(_cardsPair.IsOnePair(), "Set of cards must have a pair.");
        }

        [Test]
        public void TestCardsFlush_Fail()
        {
            Assert.IsFalse(_cardsPair.IsFlush(), "set of cards must not have a flush.");
        }
    }
}