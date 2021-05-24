using Payment.Tracker.Notifier.Email.NotificationProviders;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email
{
    public interface IEmailNotificationStrategy
    {
        IEmailNotificationHandler GetProvider(NotificationType notificationType);
    }
}