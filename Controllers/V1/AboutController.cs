using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FortisPokerCard.WebService.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "foo";
        }
    }
}
