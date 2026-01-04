using Swashbuckle.AspNetCore.Filters;
using Games.Microservice.Domain.Core;
using Games.Microservice.Application.Common;



public class GenericErrorUnauthorizedExample : IExamplesProvider<BaseResponse<object>>
{
    public BaseResponse<object> GetExamples()
    {
        var notification = new NotificationModel
        {
            NotificationType = NotificationModel.ENotificationType.Unauthorized
        };

        notification.AddMessage("Authorization", "You are not authorized to perform this action.");
        notification.AddMessage("Token", "Missing or invalid authentication token.");

        return BaseResponse<object>.Fail(notification);
    }
}
