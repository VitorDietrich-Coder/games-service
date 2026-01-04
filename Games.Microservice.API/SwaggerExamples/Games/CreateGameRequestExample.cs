using Games.Microservice.Application.Commands.CreateGame;
using Swashbuckle.AspNetCore.Filters;

namespace FGC.Api.SwaggerExamples.Games
{
    
    public class CreateGameRequestExample : IExamplesProvider<CreateGameCommand>
    {
        public CreateGameCommand GetExamples()
        {
            return new CreateGameCommand("cs2", "FPS", 90.00M);
        }
    }
}
