using FortisService.Core.Exceptions;
using FortisService.Core.Extensions;
using FortisService.Core.Models.Messages;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.Core.Services;
using FortisService.DataContext;
using FortisService.Models.Enumerator;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FortisPokerCard.WebService.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly FortisDbContext _databaseContext;
        private readonly GameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(
            FortisDbContext databaseContext,
             GameService gameService,
             ILogger<GameController> logger)
        {
            _databaseContext = databaseContext;
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new poker game.
        /// </summary>
        /// <returns>A newly created game.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/V1/Game
        ///     {
        ///       "Key": "A1",
        ///        "PlayerIds": [
        ///          1,
        ///          2,
        ///          3
        ///        ]
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created game.</response>
        /// <response code="400">Request payload bad formatted.</response>
        /// <response code="409">If the game already exist or a player is already in a inprogress game.</response>
        /// <response code="404">If some players does not exist in the DB.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
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

            var latestGameStatus = _databaseContext.StatusHistories
                .GroupBy(sh => new
                {
                    sh.GameId,
                    sh.PlayerId
                })
                .Select(g => g.ToList().OrderByDescending(sh => sh.CreatedAt).First()).ToList();

            if (latestGameStatus.Any(sh => gameEntry.PlayerIds.Contains(sh.PlayerId) && (sh.Status == Status.InProgress || sh.Status == Status.New)))
            {
                return Conflict($"A player is already in another game, can't start a new one.");
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

        /// <summary>
        /// Get a poker game.
        /// </summary>
        /// <returns>A the game by specified ID.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/V1/Game
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested game.</response>
        /// <response code="404">The requested game does not exist in the DB.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(string), 404)]
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

        /// <summary>
        /// Draw the cards for the players.
        /// </summary>
        /// <returns>Return the cards for each player in the game.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/V1/Game/{id}/Hands
        ///     
        /// </remarks>
        /// <response code="200">Return the cards for each player in the game.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">The game does not exist.</response>
        /// <response code="409">The is already in progress.</response>
        [HttpPut("{id}/Hands")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Game>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 409)]
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

        /// <summary>
        /// Get the game result.
        /// </summary>
        /// <returns>Return the cards of each player and the winner of the game.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/V1/Game/{id}/Result
        ///     
        /// </remarks>
        /// <response code="200">Return the cards of each player and the winner of the game.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">The game does not exist.</response>
        [HttpGet("{id}/Result")]
        [ProducesResponseType(typeof(CreatedResponseMessage<GameResultResponse>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<GameResultResponse>), 400)]
        [ProducesResponseType(typeof(string), 404)]
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

            IList<PlayerHandResponse> playerstResponse;
            var isGameDone = _databaseContext.StatusHistories.Any(sh => sh.Status == Status.Done && sh.GameId == routeParameters.Id);
            if (isGameDone)
            {
                playerstResponse = await _gameService.GetResultAsync(routeParameters.Id, HttpContext.RequestAborted);
                return new GameResultResponse(playerstResponse);
            }
            
            playerstResponse = await _gameService.DetermineAndGetResultAsync(routeParameters.Id, HttpContext.RequestAborted);

            return Ok(new GameResultResponse(playerstResponse));
        }
    }
}
