using System;
using System.Threading.Tasks;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email
{
    public interface IEmailSender
    {
        Task SendAsync(NotificationType notificationType, DateTime date);
    }
}