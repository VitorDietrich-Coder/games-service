
namespace Games.Microservice.API.Swagger.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerResponseProfileAttribute : Attribute
    {
        public string ProfileName { get; }

        public SwaggerResponseProfileAttribute(string profileName)
        {
            ProfileName = profileName;
        }
    }
}
