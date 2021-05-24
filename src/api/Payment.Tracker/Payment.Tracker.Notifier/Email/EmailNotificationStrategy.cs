using System;
using System.Collections.Generic;
using System.Linq;
using Payment.Tracker.Notifier.Email.NotificationProviders;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email
{
    public class EmailNotificationStrategy : IEmailNotificationStrategy
    {
        private readonly IEnumerable<IEmailNotificationHandler> _notificationProviders;

        public EmailNotificationStrategy(IEnumerable<IEmailNotificationHandler> notificationProviders)
        {
            _notificationProviders = notificationProviders;
        }
        
        public IEmailNotificationHandler GetProvider(NotificationType notificationType)
        {
            var provider = _notificationProviders.SingleOrDefault(x => x.NotificationType == notificationType);
            if (provider == null)
            {
                throw new ArgumentException($"Unable to find email notification provider for notification type {notificationType}");
            }

            return provider;
        }
    }
}