using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.Notifier.Email;
using Payment.Tracker.Notifier.Email.NotificationProviders;
using Payment.Tracker.Notifier.Models;
using Quartz;

namespace Payment.Tracker.Notifier.Jobs
{
    public class EmailNotifierJob : IJob
    {
        private readonly IEmailNotificationStrategy _emailNotificationStrategy;
        private readonly ILogger<EmailNotifierJob> _logger;

        public EmailNotifierJob(
            IEmailNotificationStrategy emailNotificationStrategy,
            ILogger<EmailNotifierJob> logger)
        {
            _emailNotificationStrategy = emailNotificationStrategy;
            _logger = logger;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Starting {nameof(EmailNotifierJob)}");
            var typeText = context.MergedJobDataMap["NotificationType"].ToString();
            if (!int.TryParse(typeText, out var type)
                || !Enum.IsDefined(typeof(NotificationType), type))
            {
                _logger.LogError("Unable to get notification type from job context");
                return;
            }

            var notificationType = (NotificationType) type;

            IEmailNotificationHandler handler = _emailNotificationStrategy.GetProvider(notificationType);
            await handler.HandleNotificationAsync();
        }
    }
}