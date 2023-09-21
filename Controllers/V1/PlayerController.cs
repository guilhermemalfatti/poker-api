using FortisService.Core.Exceptions;
using FortisService.Core.Extensions;
using FortisService.Core.Models.Messages;
using FortisService.Core.Models.Tables;
using FortisService.Core.Payload.V1;
using FortisService.DataContext;
using FortisService.Models.Models.Tables;
using FortisService.Models.Payloads;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <returns>A newly created player.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/V1/Player
        ///     {
        ///       "name": "foo"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created player.</response>
        /// <response code="400">Request payload bad formatted.</response>
        /// <response code="409">The player already exist.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResponseMessage<Player>), 200)]
        [ProducesResponseType(typeof(string), 409)]
        public async Task<ActionResult<ObjectResponseMessage<Player>>> Create(
            [FromBody] PlayerEntry playerEntry)
        {
            var player = new Player
            {
                Name = playerEntry.Name,
            };

            try
            {
                var playerEntity = await _databaseContext.CreateAsync(g => g.Name == playerEntry.Name, player, HttpContext.RequestAborted);
                return playerEntity;
            }
            catch (ConflictFortisException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Fetch a player by Id.
        /// </summary>
        /// <returns>A the requested player.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/V1/Player/{id}
        ///
        /// </remarks>
        /// <response code="200">Returns the player.</response>
        /// <response code="400">Request bad formatted.</response>
        /// <response code="404">The player was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CreatedResponseMessage<Player>), 200)]
        [ProducesResponseType(typeof(ErrorResponseMessage<Game>), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<Player>> Get([FromRoute] RouteIdParameters routeParameters)
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
