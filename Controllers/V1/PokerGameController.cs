using FortisService.Core.Extensions;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.DataContext;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FortisPokerCard.WebService.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class PokerGameController : Controller
    {
        private readonly FortisDbContext _databaseContext;

        public PokerGameController(
            FortisDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // todo create a class to represent game entry
        [HttpPost]
        public async Task<string> CreateGame(
            [FromBody] GameEntry gameEntry)
        {
            var game = new Game
            {
                Key = gameEntry.Key,
            };
            await _databaseContext.CreateAsync(g => g.Id == game.Id, game, HttpContext.RequestAborted);
            return "foo";
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
