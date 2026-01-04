using Swashbuckle.AspNetCore.Filters;
using Games.Microservice.Domain.Core;
using Games.Microservice.Application.Common;



public class GenericErrorConflictExample : IExamplesProvider<BaseResponse<object>>
{
    public BaseResponse<object> GetExamples()
    {
        var notification = new NotificationModel
        {
            NotificationType = NotificationModel.ENotificationType.BusinessRules
        };

        notification.AddMessage("Field", "Field already in use");
        notification.AddMessage("Conflict", "This resource already exists and cannot be duplicated");

        return BaseResponse<object>.Fail(notification);
    }
}
