using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Payment.Tracker.DataLayer.Models;
using Payment.Tracker.DataLayer.Repositories;
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
            var currentIds = await _paymentSetRepository.GetAsAsync(
                set => set.Id,
                set => !set.InvoicesAttached && set.ForMonth.Year == now.Year && set.ForMonth.Month == now.Month);

            if (currentIds.Count <= 0)
            {
                Logger.LogInformation("Current period closed. Exiting");
                return;
            }
            
            await SendEmailAsync(NotificationType, now);
        }
    }
}