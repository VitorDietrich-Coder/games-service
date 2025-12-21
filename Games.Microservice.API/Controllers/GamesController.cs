using Games.Microservice.Application.Commands;
using Games.Microservice.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Games.Microservice.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ApiControllerBase
    {
     

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
            => Ok(await Mediator.Send(new SearchGamesQuery(q)));

        [HttpGet("popular")]
        public async Task<IActionResult> Popular()
        {
            return Ok(await Mediator.Send(new GetPopularGamesQuery()));
        }

        [HttpGet("recommendations")]
        public async Task<IActionResult> Recommend()
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);
            return Ok(await Mediator.Send(new GetRecommendedGamesQuery(userId)));
        }


        [HttpPost("{gameId}/purchase")]
        public async Task<IActionResult> Purchase(Guid gameId)
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);

            var result = await Mediator.Send(
                new PurchaseGameCommand(gameId, userId));

            return result ? Ok() : BadRequest();
        }
    }

}
