using static Games.Microservice.Domain.Core.NotificationModel;

namespace Games.Microservice.Domain.Core
{
    public interface INotification
    {
        NotificationModel NotificationModel { get; }
        bool HasNotification { get; }
        void AddNotification(string key, string message, ENotificationType notificationType);

    }
}
