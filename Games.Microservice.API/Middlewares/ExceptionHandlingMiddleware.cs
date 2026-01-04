using System.Net;
using System.Text.Json;
using Ardalis.GuardClauses.Net9;
using Games.Microservice.Application.Common;
using Games.Microservice.Domain.Core;
using Games.Microservice.Domain.Core.Exceptions;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, "Not Found", NotificationModel.ENotificationType.NotFound, HttpStatusCode.NotFound);
        }
        catch (ValidatorException ex)
        {
            await HandleExceptionAsync(context, ex, "Validation Error", NotificationModel.ENotificationType.BadRequestError, HttpStatusCode.BadRequest);
        }
        catch (BusinessRulesException ex)
        {
            await HandleExceptionAsync(context, ex, "Business Rules Violation", NotificationModel.ENotificationType.BusinessRules, HttpStatusCode.Conflict);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, "Internal Server Error", NotificationModel.ENotificationType.InternalServerError, HttpStatusCode.InternalServerError);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string title,
        NotificationModel.ENotificationType type,
        HttpStatusCode statusCode)
    {
        var correlationId = Guid.NewGuid().ToString();
        var request = context.Request;

        logger.LogError(exception,
            "Exception: {Title}, CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}, Query: {Query}, UserAgent: {UserAgent}",
            title,
            correlationId,
            request.Path,
            request.Method,
            request.QueryString,
            request.Headers.UserAgent.ToString());

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var isDevelopment = context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true;
        var message = isDevelopment ? exception.Message : "An unexpected error occurred.";

        var notificationService = context.RequestServices.GetService<INotification>();
        var notification = notificationService?.HasNotification == true
            ? notificationService.NotificationModel
            : new NotificationModel { NotificationType = type };

        if (notification.FieldMessages.Count == 0 && notification.GeneralMessages.All(g => g.Message != message))
        {
            notification.AddMessage(title, message);
        }

        var response = BaseResponse<object>.Fail(notification);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

