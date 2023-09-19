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

        public async Task<Game> CreateGameStatusAsync(
            GameEntry gameEntry,
            Game game,
            CancellationToken cancellationToken = default)
        {
            var gameEntity = await _databaseContext.GetOrAddAsync(g => g.Key == gameEntry.Key, game).ConfigureAwait(false);

            foreach (var playerId in gameEntry.PlayerIds)
            {
                var status = new StatusHistory
                {
                    GameId = gameEntity.Id,
                    PlayerId = playerId,
                    Status = Status.New
                };

                await _databaseContext.GetOrAddAsync(sh => 
                    sh.GameId == gameEntity.Id &&
                    sh.PlayerId == playerId && 
                    sh.Status == Status.New
                , status, cancellationToken).ConfigureAwait(false);
            }


            return gameEntity;
        }
    }
}
