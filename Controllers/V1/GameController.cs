using FortisService.Core.Exceptions;
using FortisService.Core.Extensions;
using FortisService.Core.Models.Messages;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.Core.Services;
using FortisService.DataContext;
using FortisService.Models.Enumerator;
using FortisService.Models.Models.Tables;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace FortisPokerCard.WebService.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly FortisDbContext _databaseContext;
        private readonly GameService _gameService;

        public GameController(
            FortisDbContext databaseContext,
             GameService gameService)
        {
            _databaseContext = databaseContext;
            _gameService = gameService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 201)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Player>), 409)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Player>), 404)]
        public async Task<ActionResult<ObjectResponseMessage<Game>>> CreateGame(
            [FromBody] GameEntry gameEntry)
        {

            var playersCount = await _databaseContext.Players
                .Where(p => gameEntry.PlayerIds.Contains(p.Id))
                .CountAsync();

            if (playersCount != gameEntry.PlayerIds.Count)
            {
                return NotFound("A player in the PlayerIds does not exist in th DB.");
            }

            if (await _databaseContext.Games.AnyAsync(g => g.Key == gameEntry.Key))
            {
                return Conflict($"The Game with key {gameEntry.Key} already exist.");
            }
            ObjectResponseMessage<Game> gameResponse;
            try
            {
                var game = new Game
                {
                    Key = gameEntry.Key,
                };
                gameResponse = await _databaseContext.CreateAsync(g => g.Key == gameEntry.Key, game, HttpContext.RequestAborted);
            }
            catch (ConflictFortisException ex)
            {
                return Conflict(ex.Message);
            }


            await _gameService.CreateGameStatusAsync(gameEntry, gameResponse.Entity, HttpContext.RequestAborted).ConfigureAwait(false);
            return Ok(gameResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 404)]
        public async Task<ActionResult<Game>> GetGame([FromRoute] RouteIdParameters routeParameters)
        {
            try
            {
                var gameQuery = await _databaseContext.GetOrThrowAsync<Game>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted);

                return Ok(gameQuery);
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/Hands")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 404)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 409)]
        public async Task<ActionResult<IList<PlayerHandResponse>>> CreateHands([FromRoute] RouteIdParameters routeParameters)
        {
            try
            {
                await _databaseContext.GetOrThrowAsync<Game>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted);
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }

            var isGameInProgress = await _databaseContext.StatusHistories.AnyAsync(sh => sh.GameId == routeParameters.Id && sh.Status == Status.InProgress, HttpContext.RequestAborted);
            if (isGameInProgress)
            {
                return Conflict($"The game id {routeParameters.Id} is in progress already.");
            }


            var hands = await _gameService.CreateInProgressStatusAsync(routeParameters.Id, HttpContext.RequestAborted);

            return Ok(hands);
        }

        // todo
        [HttpGet("{id}/Hands")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 404)]
        public async Task<ActionResult<IList<PlayerHandResponse>>> GetPlayerHand(
            [FromRoute] RouteIdParameters routeParameters)
        {
            try
            {
                await _databaseContext.GetOrThrowAsync<Game>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted);
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }

            var hands = await _gameService.GetPlayerHandsByGameIdAsync(routeParameters.Id, HttpContext.RequestAborted);

            return Ok(hands);
        }

        // if all player have requested the cards see who won
        [HttpGet("{id}/Result")]
        [ProducesResponseType(typeof(CreatedResponseMessage<GameResultResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<GameResultResponse>), 400)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 404)]
        public async Task<ActionResult<GameResultResponse>> GetResult(
            [FromRoute] RouteIdParameters routeParameters)
        {
            try
            {
                await _databaseContext.GetOrThrowAsync<Game>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted);
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }

            var isGameDone = _databaseContext.StatusHistories.Any(sh => sh.Status == Status.Done && sh.GameId == routeParameters.Id);
            if (isGameDone)
            {
                return Ok(await _gameService.GetResultAsync(routeParameters.Id, HttpContext.RequestAborted));
            }

            var playerstResponse = await _gameService.DetermineAndGetResultAsync(routeParameters.Id, HttpContext.RequestAborted);

            var result = new GameResultResponse(playerstResponse);
            return Ok(result);
        }
    }
}
