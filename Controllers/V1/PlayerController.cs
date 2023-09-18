using FortisService.Core.Exceptions;
using FortisService.Core.Extensions;
using FortisService.Core.Models.Messages;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.DataContext;
using FortisService.Models.Models.Tables;
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
    public class PlayerController : Controller
    {
        private readonly FortisDbContext _databaseContext;

        public PlayerController(
            FortisDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // todo create a class to represent game entry
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResponseMessage<Player>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Player>), 409)]
        public async Task<ActionResult<ObjectResponseMessage<Player>>> Create(
            [FromBody] PlayerEntry playerEntry)
        {
            var player = new Player
            {
                Name = playerEntry.Name,
            };

            try
            {
                var test = await _databaseContext.CreateAsync(g => g.Name == playerEntry.Name, player, HttpContext.RequestAborted);
                return test;
            }
            catch (ConflictFortisException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> Get([FromRoute] RouteIdParameters routeParameters)
        {
            try
            {
                return Ok(await _databaseContext.GetOrThrowAsync<Player>(g => g.Id == routeParameters.Id, HttpContext.RequestAborted));
            }
            catch (NotFoundFortisException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
