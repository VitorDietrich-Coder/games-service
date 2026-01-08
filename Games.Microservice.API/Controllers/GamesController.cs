using Games.Microservice.API.Swagger.Attributes;
using Games.Microservice.Application.Commands;
using Games.Microservice.Application.Commands.CreateGame;
using Games.Microservice.Application.Games.Models.Response;
using Games.Microservice.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Games.Microservice.API.Controllers
{
    /// <summary>
    /// Manages game-related operations such as creation, retrieval, and listing.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class GamesController : ApiControllerBase
    {
        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <param name="request">Game details to be created.</param>
        /// <returns>The created game data.</returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Create game",
            Description = "Registers a new game with details such as title, genre, price, and optional promotion."
        )]
        [SwaggerResponseProfile("Games.Create")]
        public async Task<ActionResult<GameResponse>> Create([FromBody] CreateGameCommand command)
        {
            return await Mediator.Send(command);
        }
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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await Mediator.Send(new GetRecommendedGamesQuery(userId)));
        }


        [HttpPost("{gameId}/purchase")]
        public async Task<IActionResult> Purchase(Guid gameId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await Mediator.Send(
                new PurchaseGameCommand(gameId, userId));

            return result ? Ok() : BadRequest();
        }
        ///// <summary>
        ///// Lists all games.
        ///// </summary>
        ///// <returns>A list of all games.</returns>
        //[HttpGet]
        //[SwaggerOperation(
        //    Summary = "List all games",
        //    Description = "Returns a collection of all games available in the system."
        //)]

        //[SwaggerResponseProfile("Games.GetAll")]
        //public async Task<ActionResult<List<GameResponse>>> GetAll()
        //{
        //    return await Mediator.Send(new GetAllGamesQuery());
        //}

        ///// <summary>
        ///// Gets a game by its ID.
        ///// </summary>
        ///// <param name="id">Game ID.</param>
        ///// <returns>Game details if found.</returns>
        //[HttpGet("{id:int:min(1)}")]
        //[SwaggerOperation(
        //    Summary = "Get Games by ID",
        //    Description = "Returns game details for the given ID. Returns 404 if the game doesn't exist."
        //)]

        //[SwaggerResponseProfile("Games.Get")]
        //public async Task<ActionResult<GameResponse>> GetAsync(int id)
        //{
        //    return await Mediator.Send(new GetGameByIdQuery { Id = id });
        //}
    }
}
