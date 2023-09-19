using FortisService.Core.Exceptions;
using FortisService.Core.Extensions;
using FortisService.Core.Models.Messages;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.Core.Services;
using FortisService.DataContext;
using FortisService.Models.Models.Tables;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(typeof(ErrorResponseMessage<Player>), 409)]
        public async Task<ActionResult<ObjectResponseMessage<Game>>> CreateGame(
            [FromBody] GameEntry gameEntry)
        {

            try
            {
                foreach (var playerId in gameEntry.PlayerIds)
                {
                    await _databaseContext.GetOrThrowAsync<Player>(p => p.Id == playerId, HttpContext.RequestAborted);
                }
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }

            var game = new Game
            {
                Key = gameEntry.Key,
            };

            return Ok(await _gameService.CreateGameStatusAsync(gameEntry, game, HttpContext.RequestAborted).ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame([FromRoute] RouteIdParameters routeParameters)
        {
            var gameQuery = await _databaseContext.GetOrThrowAsync<Game>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted);

            return Ok(gameQuery);
            //return Ok(await gameQuery.FirstAsync());
        }

        // todo query path to identify the player
        [HttpGet("{id}/playerHand")]
        public string GetPlayerHand(
            [FromRoute] RouteIdParameters routeParameters)
        {
            return $"foo {routeParameters.Id}";
        }

        // if all player have requested the cards see who won
        [HttpGet("result")]
        public string GetResult()
        {
            return "foo";
        }
    }
}
