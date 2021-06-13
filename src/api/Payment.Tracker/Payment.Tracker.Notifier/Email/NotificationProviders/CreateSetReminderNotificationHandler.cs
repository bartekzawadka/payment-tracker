using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email.NotificationProviders
{
    public class CreateSetReminderNotificationHandler : EmailNotificationHandler, IEmailNotificationHandler
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetRepository;
        public NotificationType NotificationType => NotificationType.CreateSetReminder;
        
        public CreateSetReminderNotificationHandler(
            IGenericRepository<PaymentSet> paymentSetRepository,
            IEmailSender emailSender,
            ILogger<CreateSetReminderNotificationHandler> logger)
            : base(emailSender, logger)
        {
            _paymentSetRepository = paymentSetRepository;
        }
        
        public async Task HandleNotificationAsync()
        {
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = new DateTime(now.Year, now.Month + 1, 1);

            if (await _paymentSetRepository.ExistsAsync(new Filter<PaymentSet>(
                x => x.ForMonth >= startMonth && x.ForMonth < endMonth)))
            {
                Logger.LogInformation($"Payment set created for period {now:MM-yyyy}. Exising.");
                return;
            }

            await SendEmailAsync(NotificationType, now);
        }
    }
}