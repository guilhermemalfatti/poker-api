using FortisService.Core.Extensions;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.DataContext;
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
            [FromBody] Game game)
        {
            await _databaseContext.CreateAsync(g => g.Id == game.Id, game, HttpContext.RequestAborted);
            return "foo";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame([FromRoute] RouteIdParameters routeParameters)
        {
            var game = await _databaseContext.Games
                .Where(g => g.Id == routeParameters.Id)
                .ToListAsync();

            return Ok(game.First());
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
