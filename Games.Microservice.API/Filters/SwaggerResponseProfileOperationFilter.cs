
using Games.Microservice.API.Swagger.Attributes;
using Games.Microservice.Application.Common;
using Games.Microservice.Application.Games.Models.Response;
using Games.Microservice.Domain.Core;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace FGC.Api.Swagger.Filters
{
    public class SwaggerResponseProfileOperationFilter : IOperationFilter
    {
        private static readonly Dictionary<string, (int StatusCode, Type ResponseType, Type ExampleType)[]> Profiles
            = new()
            {    
                ["Games.Create"] = new[]
                {
                    (200, typeof(SuccessResponse<GameResponse>), typeof(SuccessResponse<GameResponse>)),
                    (400, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorBadRequestExample)),
                    (401, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorUnauthorizedExample)),
                    (403, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorForbiddenExample)),
                    (409, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorConflictExample)),
                    (500, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorInternalServerExample)),
                },
                ["Games.Get"] = new[]
                {
                    (200, typeof(SuccessResponse<GameResponse>), typeof(SuccessResponse<GameResponse>)),
                    (401, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorUnauthorizedExample)),
                    (403, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorForbiddenExample)),
                    (404, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorNotFoundExample)),
                    (500, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorInternalServerExample)),
                },
                ["Games.GetAll"] = new[]
                {
                    (200, typeof(SuccessResponse<IEnumerable<GameResponse>>), typeof(SuccessResponse<GameResponse>)),
                    (401, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorUnauthorizedExample)),
                    (403, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorForbiddenExample)),
                    (500, typeof(BaseResponse<NotificationModel>), typeof(GenericErrorInternalServerExample)),
                },
            };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attr = context.MethodInfo.GetCustomAttribute<SwaggerResponseProfileAttribute>();
            if (attr == null) return;

            if (!Profiles.TryGetValue(attr.ProfileName, out var responses))
                return;

            foreach (var (status, respType, exampleType) in responses)
            {
                var schema = context.SchemaGenerator.GenerateSchema(respType, context.SchemaRepository);
                var exampleInstance = Activator.CreateInstance(exampleType);
                var exampleValue = exampleType.GetMethod("GetExamples")?.Invoke(exampleInstance, null);

                operation.Responses[status.ToString()] = new OpenApiResponse
                {
                    Description = GetDescription(status),
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = schema,
                            Example = OpenApiAnyFactory.CreateFor(exampleValue)
                        }
                    }
                };
            }
        }

        private static string GetDescription(int status) => status switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            500 => "Internal Server Error",
            _ => "Response"
        };
    }

    public static class OpenApiAnyFactory
    {
        public static IOpenApiAny CreateFor(object example)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(example);
            return new Microsoft.OpenApi.Any.OpenApiString(json);
        }
    }
}
