using System.Threading.Tasks;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email.NotificationProviders
{
    public interface IEmailNotificationHandler
    {
        NotificationType NotificationType { get; }

        Task HandleNotificationAsync();
    }
}