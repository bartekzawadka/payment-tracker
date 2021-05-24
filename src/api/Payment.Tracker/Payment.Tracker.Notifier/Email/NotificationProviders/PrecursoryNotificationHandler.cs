using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email.NotificationProviders
{
    public class PrecursoryNotificationHandler : EmailNotificationHandler, IEmailNotificationHandler
    {
        private readonly ILogger<PrecursoryNotificationHandler> _logger;
        public NotificationType NotificationType => NotificationType.Precursory;

        public PrecursoryNotificationHandler(IEmailSender emailSender, ILogger<PrecursoryNotificationHandler> logger)
            : base(emailSender, logger)
        {
            _logger = logger;
        }

        public async Task HandleNotificationAsync()
        {
            await SendEmailAsync(NotificationType, DateTime.Now.AddMonths(1));
        }
    }
}