using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
using Payment.Tracker.DataLayer.Sys;
using Payment.Tracker.Notifier.Models;

namespace Payment.Tracker.Notifier.Email.NotificationProviders
{
    public class ClosePeriodReminderNotificationHandler : EmailNotificationHandler, IEmailNotificationHandler
    {
        private readonly IGenericRepository<PaymentSet> _paymentSetRepository;
        public NotificationType NotificationType => NotificationType.ClosePeriodReminder;
        
        public ClosePeriodReminderNotificationHandler(
            IGenericRepository<PaymentSet> paymentSetRepository,
            IEmailSender emailSender,
            ILogger<ClosePeriodReminderNotificationHandler> logger)
            : base(emailSender, logger)
        {
            _paymentSetRepository = paymentSetRepository;
        }
        
        public async Task HandleNotificationAsync()
        {
            var now = DateTime.Now;
            var startMonth = new DateTime(now.Year, now.Month, 1);
            var endMonth = new DateTime(now.Year, now.Month + 1, 1);
            var currentIds = await _paymentSetRepository.GetAllAsAsync(set => set.Id,
                new Filter<PaymentSet>(set => (set.PaymentPositions.Any(x => !x.Paid)
                                               || !set.InvoicesAttached)
                                              && set.ForMonth >= startMonth
                                              && set.ForMonth < endMonth));

            if (currentIds.Count <= 0)
            {
                Logger.LogInformation("Current period closed. Exiting");
                return;
            }
            
            await SendEmailAsync(NotificationType, now);
        }
    }
}