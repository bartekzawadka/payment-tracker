using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email.NotificationProviders
{
    public abstract class EmailNotificationHandler
    {
        private readonly IEmailSender _emailSender;
        protected readonly ILogger Logger;

        protected EmailNotificationHandler(IEmailSender emailSender, ILogger logger)
        {
            _emailSender = emailSender;
            Logger = logger;
        }

        protected async Task SendEmailAsync(NotificationType notificationType, DateTime date)
        {
            try
            {
                Logger.LogInformation($"Sending email with {notificationType} notification");
                await _emailSender.SendAsync(notificationType, date);
                Logger.LogInformation($"{notificationType} notification email sent for date {date:MM-yyyy}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex , $"Unable to send {notificationType} notification email for date {date:MM-yyyy}");
                throw;
            }
        }
    }
}