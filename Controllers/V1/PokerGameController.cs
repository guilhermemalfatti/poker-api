using FortisService.Core.Extensions;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.DataContext;
using Microsoft.AspNetCore.Mvc;

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
            [FromBody] int gameId,
            [FromBody] string name)
        {
            var game = new Game
            {
                Id = gameId,
                Name = "test"
            };
            await _databaseContext.CreateAsync(g => g.Id == gameId, game, HttpContext.RequestAborted);
            return "foo";
        }

        [HttpGet("{id}")]
        public String GetGame([FromRoute] RouteIdParameters routeParameters)
        {
            return "foo";
        }

        // todo query path to identify the player
        [HttpGet("{id}/playerHand")]
        public String GetPlayerHand(
            [FromRoute] RouteIdParameters routeParameters)
        {
            return $"foo {routeParameters.Id}";
        }

        // if all player have requested the cards see who won
        [HttpGet("result")]
        public String GetResult()
        {
            return "foo";
        }
    }
}
