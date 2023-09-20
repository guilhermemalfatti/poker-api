using FortisService.DataContext;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using FortisService.Core.Models.Tables;
using FortisService.Core.Extensions;
using FortisService.Models.Payloads;
using FortisService.Core.Models.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FortisService.Models.Models.Tables;
using FortisService.Models.Enumerator;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FortisService.Core.Services
{
    public class GameService
    {
        private readonly FortisDbContext _databaseContext;
        private readonly ILogger<GameService> _logger;

        public GameService(
            FortisDbContext databaseContext,
            ILogger<GameService> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public async Task CreateGameStatusAsync(
            GameEntry gameEntry,
            Game gameEntity,
            CancellationToken cancellationToken = default)
        {
            await CreateStatusHistoriesAsync(gameEntry.PlayerIds, gameEntity.Id, Status.New, cancellationToken);
            return;
        }

        private async Task CreateStatusHistoriesAsync(
            IList<int> playerIds,
            int gameId,
            Status status,
            CancellationToken cancellationToken = default)
        {
            foreach (var playerId in playerIds)
            {
                var statusHistory = new StatusHistory
                {
                    GameId = gameId,
                    PlayerId = playerId,
                    Status = status,
                };
                await CreateStatusHistory(statusHistory, cancellationToken).ConfigureAwait(false);
            }
            return;
        }

        private async Task<StatusHistory> CreateStatusHistory(
            StatusHistory statusHistory,
            CancellationToken cancellationToken = default)
        {
            var sh = await _databaseContext.GetOrAddAsync(sh =>
                sh.GameId == statusHistory.GameId &&
                sh.PlayerId == statusHistory.PlayerId &&
                sh.Status == statusHistory.Status
            , statusHistory, cancellationToken).ConfigureAwait(false);

            return sh;
        }

        public async Task<IList<PlayerHandResponse>> CreateInProgressStatusAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            var cards = await _databaseContext.Cards.ToListAsync(cancellationToken).ConfigureAwait(false);

            var seed = DateTime.UtcNow.Millisecond;
            Random random = new Random(seed);
            var shuffledCards = cards.OrderBy(c => random.Next()).ToList();

            var playerStatusHistoriesForCurrentGame = await _databaseContext.StatusHistories
                .Include(sh => sh.Player)
                .Where(sh => sh.GameId == gameId && sh.Status == Status.New)
                .Select(sh => sh)
                .ToListAsync(cancellationToken);

            var playerHandsResponse = new List<PlayerHandResponse>();

            var indexCount = 0;
            foreach (var psh in playerStatusHistoriesForCurrentGame)
            {
                var playerCards = shuffledCards.GetRange(0 + indexCount, 5);

                var statusHistory = new StatusHistory
                {
                    GameId = gameId,
                    PlayerId = psh.Player.Id,
                    Status = Status.InProgress,
                    FirstCardId = playerCards[0].Id,
                    SecondCardId = playerCards[1].Id,
                    ThirdCardId = playerCards[2].Id,
                    FourthCardId = playerCards[3].Id,
                    FifthCardId = playerCards[4].Id,
                };
                await CreateStatusHistory(statusHistory, cancellationToken).ConfigureAwait(false);

                playerHandsResponse.Add(new PlayerHandResponse(psh.Player, playerCards));

                indexCount += 5;
            }
            return playerHandsResponse;
        }

        private async Task<IList<StatusHistory>> getInProgressPlayerStatusHisotryAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            return await _databaseContext.StatusHistories
                .Include(sh => sh.Player)
                .Include(sh => sh.FirstCard)
                .Include(sh => sh.SecondCard)
                .Include(sh => sh.ThirdCard)
                .Include(sh => sh.FourthCard)
                .Include(sh => sh.FifthCard)
                .Where(sh => sh.GameId == gameId && sh.Status == Status.InProgress)
                .Select(sh => sh)
                .ToListAsync(cancellationToken);
        }

        public async Task<IList<PlayerHandResponse>> DetermineAndGetResultAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            var playerStatusHistoriesForCurrentGame = await getInProgressPlayerStatusHisotryAsync(gameId, cancellationToken).ConfigureAwait(false);

            var playersHandResponse = getOrderedPlayersHand(playerStatusHistoriesForCurrentGame);

            var higherHand = playersHandResponse.First().HandType;

            // winner by high hand
            var candidateWinners = playersHandResponse.Where(p => p.HandType == higherHand && p.HandType != PokerHandType.HighCard);
            if (candidateWinners.Count() == 1)
            {
                candidateWinners.First().IsWinner = true;
                await terminateGameAsync(gameId, playersHandResponse, cancellationToken);
                return playersHandResponse;
            }
            else if (candidateWinners.Any())
            {
                var candidatesSameHands = true;
                var firstCandidate = candidateWinners.FirstOrDefault();
                foreach (var candidateWinner in candidateWinners)
                {
                    var result = firstCandidate.CardList.Intersect(candidateWinner.CardList);
                    if (result.Count() == candidateWinner.CardList.Count())
                    {
                        candidatesSameHands = false;
                    }
                }

                if (!candidatesSameHands)
                {
                    winnerByTieHighHand(gameId, candidateWinners, playersHandResponse, cancellationToken);
                    await terminateGameAsync(gameId, playersHandResponse, cancellationToken);
                    return playersHandResponse;
                }
            }

            if (!playersHandResponse.All(p => p.HandType == null))
            {
                winnerByHighHand(playersHandResponse, cancellationToken);
            }
                
            // at this point the game is a tie
            await terminateGameAsync(gameId, playersHandResponse, cancellationToken);

            return playersHandResponse;
        }

        private void winnerByTieHighHand(
            int gameId, 
            IEnumerable<PlayerHandResponse> candidateWinners,
            IList<PlayerHandResponse>  playersHandResponse,
            CancellationToken cancellationToken = default)
        {
            var candidateWinner = new PlayerHandResponse();

            var groupCandidatesByHandTypeAndRank = candidateWinners.GroupBy(p => new 
            {
                p.HandType,
                p.GetGetHighestCardByHandType().Rank
            });

            // same handtype and same rank, in this case we need to check the highest card
            if (groupCandidatesByHandTypeAndRank.Count() == 1)
            {
                foreach (var group in groupCandidatesByHandTypeAndRank)
                {
                    foreach (var player in group.ToList())
                    {
                        player.HighCardList = player.GetHighCardList();
                    }
                }

                // todo check if 2 players have the same rank
                var candidateWinnerPlayers = groupCandidatesByHandTypeAndRank.SelectMany(g => g).OrderByDescending(p => p.HighCard.Rank);
                candidateWinner = candidateWinnerPlayers.First();
                if (candidateWinnerPlayers.Where(c => c.HighCard.Rank == candidateWinner.HighCard.Rank).Count() > 1)
                {
                    return;//it's a tie                
                }
                candidateWinner.IsWinner = true;
                return;
            }

            // handle two+ players with the same hand type and different ranks
            foreach (var group in groupCandidatesByHandTypeAndRank)
            {
                foreach (var player in group.ToList())
                {
                    player.HighCardByHandType = player.GetGetHighestCardByHandType();
                    //var highestPLayerCardByRank = player.CardList.GroupBy(c => c.Rank).OrderByDescending(g => g.Key).First().First();
                    //player.HandTypeRank = highestPLayerCardByRank.Rank;
                }
            }

            var candidatePlayers = groupCandidatesByHandTypeAndRank.SelectMany(g => g).OrderByDescending(p => p.HighCardByHandType.Rank);
            candidateWinner = candidatePlayers.First();
            if (candidatePlayers.Where(c => c.HighCardByHandType.Rank == candidateWinner.HighCardByHandType.Rank).Count() > 1)
            {
                return;//it's a tie                
            }
            candidateWinner.IsWinner = true;            
        }

        private void winnerByHighHand(IList<PlayerHandResponse> playersHandResponse, CancellationToken cancellationToken = default)
        {
            foreach (var player in playersHandResponse)
            {
                // return only highcards
                var cardGroup = player.CardList.GroupBy(c => c.Rank).Where(g => g.Count() == 1);

                // fetch the cards that belong to a single group
                player.HighCardList = cardGroup.SelectMany(g => g.ToList()).ToList();
            }

            // Winner by high card
            var groupOrderedByHighCard = playersHandResponse.GroupBy(p => p.HighCard.Rank).OrderByDescending(g => g.Key);
            if (groupOrderedByHighCard.First().Count() == 1)
            {
                var player = groupOrderedByHighCard.First().ToList().First();
                player.IsWinner = true;
            }            
        }

        public async Task<IList<PlayerHandResponse>> GetResultAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {

            var playerStatusHistoriesForCurrentGame = await getInProgressPlayerStatusHisotryAsync(gameId, cancellationToken).ConfigureAwait(false);

            var playersHand = new List<PlayerHandResponse>();


            foreach (var psh in playerStatusHistoriesForCurrentGame)
            {
                var playerCards = new List<Card> { psh.FirstCard, psh.SecondCard, psh.ThirdCard, psh.FourthCard, psh.FifthCard };
                playersHand.Add(new PlayerHandResponse(psh.Player, playerCards, psh.Winner));
            }

            return playersHand.OrderBy(p => p.CardList).ToList();
        }

        private async Task terminateGameAsync(int gameId, IList<PlayerHandResponse> playerResponses, CancellationToken cancellationToken = default)
        {
            foreach (var playerResponse in playerResponses)
            {
                var statusHistory = new StatusHistory
                {
                    GameId = gameId,
                    PlayerId = playerResponse.Player.Id,
                    Status = Status.Done,
                    Winner = playerResponse.IsWinner,
                    FirstCardId = playerResponse.CardList[0].Id,
                    SecondCardId = playerResponse.CardList[1].Id,
                    ThirdCardId = playerResponse.CardList[2].Id,
                    FourthCardId = playerResponse.CardList[3].Id,
                    FifthCardId = playerResponse.CardList[4].Id,
                };
                await CreateStatusHistory(statusHistory, cancellationToken).ConfigureAwait(false);
            }
        }

        private IList<PlayerHandResponse> getOrderedPlayersHand(IList<StatusHistory> playerStatusHistoriesForCurrentGame)
        {
            var playersHand = new List<PlayerHandResponse>();


            foreach (var psh in playerStatusHistoriesForCurrentGame)
            {
                var playerCards = new List<Card> { psh.FirstCard, psh.SecondCard, psh.ThirdCard, psh.FourthCard, psh.FifthCard };
                var handType = GetHandType(playerCards);
                playersHand.Add(new PlayerHandResponse(psh.Player, playerCards, handType));
            }

            return playersHand.OrderByDescending(p => p.HandType).ToList();
        }


        private PokerHandType GetHandType(IList<Card> cards)
        {
            // todo do I need to sort
            //.Sort((a, b) => b.Rank.CompareTo(a.Rank));

            if (cards.IsRoyalFlush())
                return PokerHandType.RoyalFlush;
            else if (cards.IsStraightFlush())
                return PokerHandType.StraightFlush;
            else if (cards.IsFourOfAKind())
                return PokerHandType.FourOfKind;
            else if (cards.IsFullHouse())
                return PokerHandType.FullHouse;
            else if (cards.IsFlush())
                return PokerHandType.Flush;
            else if (cards.IsStraight())
                return PokerHandType.Straight;
            else if (cards.IsThreeOfAKind())
                return PokerHandType.ThreeOfKind;
            else if (cards.IsTwoPair())
                return PokerHandType.TwoPair;
            else if (cards.IsOnePair())
                return PokerHandType.Pair;
            else
                return PokerHandType.HighCard;
        }
    }
}
